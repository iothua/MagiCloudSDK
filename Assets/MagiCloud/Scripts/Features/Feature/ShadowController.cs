using UnityEngine;
using System.Collections;

namespace MagiCloud.Features
{
    public enum ShadowType
    {
        Auto,
        Manual
    }

    /// <summary>
    /// 虚影控制
    /// </summary>
    public class ShadowController : MonoBehaviour
    {
        //网格数据
        private MeshFilter[] _meshFilters;
        private MeshRenderer[] _meshRenderers;
        private SkinnedMeshRenderer[] _skinnedMeshRenderers;
        //虚影透明度
        private float Intension = 0.25f;
        private Shader ghostShader;

        //渲染层级
        public int renderQueue = 3000;

        /// <summary>
        /// 虚影节点
        /// </summary>
        private Transform _traShadowNode;

        public ShadowType shadowType = ShadowType.Auto;

        /// <summary>
        /// 模型节点
        /// </summary>
        public Transform traModelNode;

        void Start()
        {
            if (shadowType == ShadowType.Manual)
            {
                if (traModelNode == null)
                    traModelNode = transform.parent;
                else
                    CreateGhostModels(traModelNode);

                CloseGhost();
            }

        }

        private void OnDestroy()
        {
            DestroyGhostModels();
        }


        /// <summary>
        /// 创建虚影模型
        /// </summary>
        /// <param name="modelNode"></param>
        public void CreateGhostModels(Transform modelNode)
        {
            if (_traShadowNode != null) return;

            //创建虚影模型
            CreateGhost(modelNode);
        }

        public void DestroyGhostModels()
        {
            if (_traShadowNode != null)
                Destroy(_traShadowNode.gameObject);
        }

        /// <summary>
        /// 创建虚影节点
        /// </summary>
        private void CreateGhostParent()
        {
            if (_traShadowNode == null)
            {
                _traShadowNode = new GameObject("Ghost").transform;
                _traShadowNode.SetParent(this.transform);
                _traShadowNode.localPosition = Vector3.zero;
                _traShadowNode.localRotation = Quaternion.identity;
                _traShadowNode.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// 创建模型虚影
        /// </summary>
        /// <param name="modelNode">模型</param>
        private void CreateGhost(Transform modelNode)
        {

            ghostShader = Shader.Find("Legacy Shaders/Transparent/Diffuse");

            _traShadowNode = CreateShadowObject(null, modelNode.transform).transform;

            _traShadowNode.localPosition = modelNode.localPosition;
            _traShadowNode.localRotation = modelNode.localRotation;
            _traShadowNode.localScale = modelNode.localScale;

            CreateNode(modelNode, _traShadowNode);

            //获取到模型的所有节点
            //然后生成Null的
        }

        /// <summary>
        /// 创建虚影物体
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        GameObject CreateShadowObject(Transform parent,Transform child)
        {
            var meshFilter = child.GetComponent<MeshFilter>();
            var meshRenderer = child.GetComponent<MeshRenderer>();

            var skinnedMesh = child.GetComponent<SkinnedMeshRenderer>();

            GameObject go;

            if (meshFilter != null && meshRenderer != null)
            {
                Mesh meshF = new Mesh();
                meshF = meshFilter.sharedMesh;

                go = CreateMesh(parent, meshF, meshFilter.transform, meshRenderer.sharedMaterials, child.name);
            }
            else if (skinnedMesh != null)
            {
                Mesh meshS = new Mesh();
                meshS = skinnedMesh.sharedMesh;

                go = CreateMesh(parent, meshS, skinnedMesh.transform, skinnedMesh.materials, child.name);
            }
            else
            {
                //都为Null
                go = new GameObject(child.name);

                if (parent != null)
                    go.transform.SetParent(parent);

                go.transform.localScale = child.transform.localScale;
                go.transform.localPosition = child.transform.localPosition;
                go.transform.localRotation = child.transform.localRotation;
            }

            return go;
        }

        /// <summary>
        /// 创建节点，最初与最终
        /// </summary>
        /// <param name="firstNode"></param>
        /// <param name="lastNode"></param>
        private void CreateNode(Transform firstNode, Transform lastNode)
        {
            foreach (Transform child in firstNode)
            {
                var go = CreateShadowObject(lastNode, child);

                CreateNode(child, go.transform);
            }
        }

        /// <summary>
        /// 创建mesh改shader
        /// </summary>
        /// <param passiveName="mesh"></param>
        /// <param passiveName="tra"></param>
        /// <param passiveName="materials"></param>
        /// <param passiveName="name"></param>
        private GameObject CreateMesh(Transform parent, Mesh mesh, Transform tra, Material[] materials, string name = "")
        {
            GameObject go = new GameObject(name);

            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            MeshRenderer meshRen = go.AddComponent<MeshRenderer>();

            meshRen.materials = materials;

            for (int i = 0; i < meshRen.materials.Length; i++)
            {
                meshRen.materials[i].shader = ghostShader;//设置xray效果
                meshRen.materials[i].renderQueue = renderQueue;
                meshRen.materials[i].SetColor("_Color", new Color(1f, 1f, 1f, Intension));
            }

            if (parent != null)
                go.transform.SetParent(parent);

            go.transform.localScale = tra.transform.localScale;
            go.transform.localPosition = tra.transform.localPosition;
            go.transform.localRotation = tra.transform.localRotation;

            return go;
        }

        /// <summary>
        /// 打开虚影
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="position"></param>
        /// <param name="rotate"></param>
        public void OpenGhost(Transform targetPos, Vector3 position = default(Vector3), Vector3 scale = default(Vector3), Quaternion rotate = default(Quaternion))
        {
            if (shadowType == ShadowType.Auto)
                CreateGhostModels(targetPos);

            _traShadowNode.parent = transform;
            _traShadowNode.localPosition = position;
            _traShadowNode.localRotation = rotate;
            //_traShadowNode.localScale = scale;
            if (_traShadowNode.gameObject.activeSelf == false)
                _traShadowNode.gameObject.SetActive(true);
        }

        /// <summary>
        /// 关闭虚影
        /// </summary>
        public void CloseGhost()
        {

            if (shadowType == ShadowType.Auto)
                DestroyGhostModels();
            else
            {
                if (_traShadowNode.gameObject.activeSelf)
                    _traShadowNode.gameObject.SetActive(false);

                _traShadowNode.SetParent(this.transform);
                _traShadowNode.transform.localPosition = Vector3.zero;
                _traShadowNode.transform.localRotation = Quaternion.identity;
                _traShadowNode.transform.localScale = Vector3.one;
            }

        }
    }
}

