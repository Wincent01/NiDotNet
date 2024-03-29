#if UNITY_5_4_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NiDotNet.NIF;
using NiDotNet.NIF.Enums;
using NiDotNet.NIF.Nodes;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "nif")]
public class NifImporter : ScriptedImporter
{
    private AssetImportContext _context;

    private NiFile _niFile;
    
    private Transform _animationRoot;
    
    private readonly Dictionary<NiObject, GameObject> _unityObjects = new Dictionary<NiObject, GameObject>();

    private readonly List<SkinMesh> _skinMeshes = new List<SkinMesh>();
    
    private struct SkinMesh
    {
        public NiSkinInstance SkinInstance;

        public GameObject Holder;
    }
    
    public override void OnImportAsset(AssetImportContext ctx)
    {
        _context = ctx;
        _niFile = new NiFile(ctx.assetPath);
        
        Debug.ClearDeveloperConsole();

        BuildHierarchy(_niFile[0] as NiNode, null);

        BuildAnimations();
        
        if (_skinMeshes.Count > 0)
        {
            FixBones();
            foreach (var mesh in _skinMeshes)
            {
                BuildRig(mesh.SkinInstance, mesh.Holder);
            }
        }

        _niFile.Dispose();
    }

    private void BuildHierarchy(NiNode node, GameObject parent)
    {
        var obj = SpawnAvObject(node, parent);
        _unityObjects.Add(node, obj);

        if (_animationRoot == null) _animationRoot = obj.transform;
        
        foreach (var child in FromIntArray(node.Children))
        {
            switch (child)
            {
                case NiNode childNode:
                    BuildHierarchy(childNode, obj);
                    break;
                case NiAVObject childAvObject:
                    SpawnAvObject(childAvObject, obj);
                    break;
                default:
                    Debug.Log($"Unhandled NiObject: {child}");
                    break;
            }
        }
    }

    private void FixBones()
    {
        //
        //    I have no clue how to properly bind the skin to the bones.
        //    This is my best attempt at getting bone positions to their correct binding positions.
        //
        foreach (var controller in _niFile.Blocks.OfType<NiTransformController>())
        {
            var target = _unityObjects[controller.Target.Object];

            if (!(controller.Interpolator.Object is NiTransformInterpolator interpolator)) continue;

            target.transform.localPosition = interpolator.Transform.Translation;
            target.transform.localRotation = interpolator.Transform.Rotation;
        }
    }

    private void BuildAnimations()
    {
        var clip = new AnimationClip();
        foreach (var block in _niFile.Blocks.OfType<NiTransformController>())
        {
            BuildAnimationGraph(block, clip);
        }

        clip.name = "Default Animation";

        var animation = _animationRoot.gameObject.AddComponent<Animation>();
        animation.AddClip(clip, clip.name);
        animation.wrapMode = WrapMode.Loop;
        animation.playAutomatically = true;
        animation.clip = clip;
        
        _context.AddObjectToAsset(clip.name, clip);
    }
    
    private void BuildAnimationGraph(NiSingleInterpController controller, AnimationClip clip)
    {
        var target = _unityObjects[controller.Target.Object];

        if (!(controller.Interpolator.Object is NiTransformInterpolator interpolator)) return;
        
        target.transform.localPosition = interpolator.Transform.Translation;
        target.transform.localRotation = interpolator.Transform.Rotation;

        if (interpolator.Data.Key == -1) return;

        var data = interpolator.Data.Object as NiKeyframeData;
        
        var path = GetRelativeKeyPath(target.transform);
        switch (data.RotationType)
        {
            case KeyType.LinearKey:

                #region Pos

                var posXCurve = new AnimationCurve();
                var posXCurveKeys = new Keyframe[data.Translations.KeyCount];
                var posYCurve = new AnimationCurve();
                var posYCurveKeys = new Keyframe[data.Translations.KeyCount];
                var posZCurve = new AnimationCurve();
                var posZCurveKeys = new Keyframe[data.Translations.KeyCount];

                for (var index = 0; index < data.Translations.Keys.Length; index++)
                {
                    var key = data.Translations.Keys[index];
                    posXCurveKeys[index] = new Keyframe(key.Time, key.Value.X, 0, 0);
                    posXCurveKeys[index] = new Keyframe(key.Time, key.Value.Y, 0, 0);
                    posXCurveKeys[index] = new Keyframe(key.Time, key.Value.Z, 0, 0);
                }

                posXCurve.keys = posXCurveKeys;
                posYCurve.keys = posYCurveKeys;
                posZCurve.keys = posZCurveKeys;

                clip.SetCurve(path, typeof(Transform), "m_LocalPosition.x", posXCurve);
                clip.SetCurve(path, typeof(Transform), "m_LocalPosition.y", posYCurve);
                clip.SetCurve(path, typeof(Transform), "m_LocalPosition.z", posZCurve);

                #endregion

                #region Rot

                var rotXCurve = new AnimationCurve();
                var rotXCurveKeys = new Keyframe[data.QuaternionKeys.Length];
                var rotYCurve = new AnimationCurve();
                var rotYCurveKeys = new Keyframe[data.QuaternionKeys.Length];
                var rotZCurve = new AnimationCurve();
                var rotZCurveKeys = new Keyframe[data.QuaternionKeys.Length];
                var rotWCurve = new AnimationCurve();
                var rotWCurveKeys = new Keyframe[data.QuaternionKeys.Length];

                for (var index = 0; index < data.QuaternionKeys.Length; index++)
                {
                    var key = data.QuaternionKeys[index];
                    rotXCurveKeys[index] = new Keyframe(key.Time, key.Value.X, 0, 0);
                    rotYCurveKeys[index] = new Keyframe(key.Time, key.Value.Y, 0, 0);
                    rotZCurveKeys[index] = new Keyframe(key.Time, key.Value.Z, 0, 0);
                    rotWCurveKeys[index] = new Keyframe(key.Time, key.Value.W, 0, 0);
                }

                rotWCurve.keys = rotWCurveKeys;
                rotYCurve.keys = rotYCurveKeys;
                rotXCurve.keys = rotXCurveKeys;
                rotZCurve.keys = rotZCurveKeys;

                clip.SetCurve(path, typeof(Transform), "m_LocalRotation.x", rotXCurve);
                clip.SetCurve(path, typeof(Transform), "m_LocalRotation.y", rotYCurve);
                clip.SetCurve(path, typeof(Transform), "m_LocalRotation.z", rotZCurve);
                clip.SetCurve(path, typeof(Transform), "m_LocalRotation.w", rotWCurve);

                #endregion

                break;
            case KeyType.QuadraticKey:
                break;
            case KeyType.TbcKey:
                break;
            case KeyType.XyzRotationKey:
                break;
            case KeyType.ConstKey:
                break;
            case KeyType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        clip.legacy = true;
    }

    private void BuildRig(NiSkinInstance skinInstance, GameObject holder)
    {
        var mesh = holder.GetComponent<MeshFilter>().sharedMesh;
        
        var partitions = skinInstance.SkinPartition.Object;

        var oldRenderer = holder.GetComponent<MeshRenderer>();
        var mat = oldRenderer.sharedMaterial;
        DestroyImmediate(oldRenderer);
        
        var renderer = holder.AddComponent<SkinnedMeshRenderer>();
        renderer.sharedMaterial = mat;
        renderer.sharedMesh = mesh;
        renderer.rootBone = _unityObjects[skinInstance.Root.Object].transform;
        
        var boneWeights = new BoneWeight[mesh.vertices.Length];
        
        //
        //    Loop through all the skin partitions to build the final rig.
        //    Currently there are skinned meshes where weird parts are stuck to root create awful rigs.
        //
        
        foreach (var partition in partitions.SkinPartitions)
        {
            for (var i = 0; i < partition.VertexMap.Length; i++)
            {
                var index = partition.VertexMap[i];
                var weight = new BoneWeight();

                for (var j = 0; j < partition.WeightsPerVertex; j++)
                { 
                    var value = partition.BoneIndices[i, j];

                    switch (j)
                    {
                        case 0:
                            weight.boneIndex0 = value;
                            break;
                        case 1:
                            weight.boneIndex1 = value;
                            break;
                        case 2:
                            weight.boneIndex2 = value;
                            break;
                        case 3:
                            weight.boneIndex3 = value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"{_context.assetPath} has a skinned mesh with more than 4 weights/vertex. This is not supported.");
                    }

                    var vertexWeight = partition.VertexWeights[i, j];
                    switch (j)
                    {
                        case 0:
                            weight.weight0 = vertexWeight;
                            break;
                        case 1:
                            weight.weight1 = vertexWeight;
                            break;
                        case 2:
                            weight.weight2 = vertexWeight;
                            break;
                        case 3:
                            weight.weight3 = vertexWeight;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"{_context.assetPath} has a skinned mesh with more than 4 weights/vertex. This is not supported.");
                    }
                }

                boneWeights[index] = weight;
            }
        }
        
        mesh.boneWeights = boneWeights;

        var bones = new Transform[skinInstance.BonesCount];
        var poses = new Matrix4x4[skinInstance.BonesCount];
        
        for (var index = 0; index < skinInstance.BonesCount; index++)
        {
            var bone = skinInstance.SkinningData.Object.BoneList[index].Transform;
            
            var boneData = skinInstance.Bones[index];
            var unityObject = _unityObjects[boneData].transform;
            bones[index] = unityObject;

            //
            //    I have no clue how to properly bind the skin to the bones.
            //    bone.Position & bone.Rotation is suppose to be offsets with the description of:
            //    "Offset of the skin from this bone in bind position."
            //    But I'm having a hard type parsing that.
            //
            
            var transform = renderer.rootBone.transform;
            
            var pos = transform.localPosition;
            var rot = transform.localEulerAngles;
            
            transform.localPosition += bone.Position;
            transform.localEulerAngles += MatrixToEulerAngles(bone.Rotation);
            
            poses[index] = unityObject.worldToLocalMatrix * renderer.rootBone.localToWorldMatrix;

            transform.localPosition = pos;
            transform.localEulerAngles = rot;
        }

        mesh.bindposes = poses;
        renderer.bones = bones;
    }

    private GameObject SpawnAvObject(NiAVObject avObject, GameObject parent)
    {
        var obj = new GameObject(avObject.Name);
        obj.transform.parent = parent ? parent.transform : null;
        obj.transform.localPosition = avObject.Position;
        obj.transform.localScale = Vector3.one * avObject.UniformScale;
        obj.transform.localEulerAngles = MatrixToEulerAngles(avObject.Rotation);

        switch (avObject)
        {
            case NiTriShape triShape:
                BuildNormalMesh(triShape, obj);
                break;
            case NiCamera camera:
                BuildCamera(camera, obj);
                break;
            case NiLODNode lodNode:
                BuildLevelsOfDetail(lodNode, obj);
                break;
        }
        
        _context.AddObjectToAsset(obj.name, obj);
        return obj;
    }

    private void BuildNormalMesh(NiGeometry triShape, GameObject holder)
    {
        if (!(triShape.Data.Object is NiTriShapeData data))
            throw new Exception($"{_context.assetPath} has invalid NiTriShapeData.");
        
        var mesh = new Mesh
        {
            name = triShape.Name,
            vertices = data.HasVertices
                ? data.Vertices.Select(s => (Vector3) s).ToArray()
                : null,
            triangles = data.HasTriangles
                ? data.Triangles.SelectMany(s => s != null ? (int[]) s : new int[0]).ToArray()
                : null,
            normals = data.HasNormals
                ? data.Normals.Select(s => (Vector3) s).ToArray() 
                : null,
            colors = data.HasVertexColors
                ? data.VertexColors.Select(s => new Color(s.R, s.G, s.B, s.A)).ToArray()
                : null,
            tangents = data.Tangents?.Select(s => (Vector4) (Vector3) s).ToArray(),
            bounds = new Bounds(data.Center, Vector3.one * data.Radius)
        };
        
        for (var i = 0; i < data.Uv.Length; i++)
        {
            var uvLayer = data.Uv[i];

            mesh.SetUVs(i, uvLayer.Select(s => (Vector2) s).ToList());
        }

        var filter = holder.AddComponent<MeshFilter>();
        
        var renderer = holder.AddComponent<MeshRenderer>();
        RegisterRenderer(renderer);
        
/*
        renderer.sharedMaterial = new Material(Shader.Find("Nif/VertexColor"));
        _context.AddObjectToAsset($"Lit {holder.name}", renderer.sharedMaterial);
*/        
        filter.mesh = mesh;

        _context.AddObjectToAsset(mesh.name, mesh);

        if (triShape.SkinInstance.Key != -1)
        {
            _skinMeshes.Add(new SkinMesh
            {
                Holder = holder,
                SkinInstance = triShape.SkinInstance.Object
            });
        }
    }

    private void BuildCamera(NiObjectNet camera, GameObject holder)
    {
        var cam = holder.AddComponent<Camera>();

        _context.AddObjectToAsset(camera.Name, cam);
    }

    private static void BuildLevelsOfDetail(NiLODNode lodNode, GameObject holder)
    {
        var data = (NiRangeLODData) lodNode.Data;

        var lod = holder.AddComponent<LODGroup>();
        
        lod.SetLODs(data.Ranges.Select(l => new LOD(1f / l.Far, null)).ToArray());
    }

    private static void RegisterRenderer(Renderer renderer)
    {
        var lod = renderer.transform.parent.GetComponent<LODGroup>();
        if (!lod) return;
        var id = int.Parse(renderer.transform.name.Split('_')[1]);

        var levels = lod.GetLODs();
        levels[id].renderers = new[] {renderer};
        lod.SetLODs(levels);
    }
    
    private static Vector3 MatrixToEulerAngles(NiMatrix3X3 matrix3X3)
    {
        //
        //    Feel free to correct this math.
        //
        
        var sy = Mathf.Sqrt(Mathf.Pow(matrix3X3.m11, 2) + Mathf.Pow(matrix3X3.m21, 2));

        var singular = sy < 1e-6;

        Vector3 angles;
        
        if (!singular)
        {
            angles = new Vector3
            {
                x = Mathf.Atan2(matrix3X3.m32, matrix3X3.m33),
                y = Mathf.Atan2(-matrix3X3.m31, sy),
                z = Mathf.Atan2(matrix3X3.m21, matrix3X3.m11)
            };
        }
        else
        {
            angles = new Vector2
            {
                x = Mathf.Atan2(-matrix3X3.m23, matrix3X3.m22),
                y = Mathf.Atan2(-matrix3X3.m31, sy)
            };
        }

        return angles * 180;
    }

    private string GetRelativeKeyPath(Transform source)
    {
        var path = "";

        var current = source;

        while (current != _animationRoot && current != null)
        {
            path = $"{current.name}/{path}";
            current = current.parent;
            if (current == _animationRoot) break;
        }

        path = path.Remove(path.Length - 1);
        path = $"/{Path.GetFileNameWithoutExtension(_context.assetPath)}/{path}";
        
        return path;
    }

    private IEnumerable<NiObject> FromIntArray(IReadOnlyList<int> array)
    {
        var list = new NiObject[array.Count];

        for (var i = 0; i < array.Count; i++)
        {
            list[i] = _niFile[array[i]];
        }

        return list;
    }
}

#endif