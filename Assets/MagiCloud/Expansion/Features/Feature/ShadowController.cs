using UnityEngine;
using System.Collections;
using HighlightingSystem;

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
    public class ShadowController :MonoBehaviour
    {
        //网格数据
        private MeshFilter[] _meshFilters;
        private MeshRenderer[] _meshRenderers;
        private SkinnedMeshRenderer[] _skinnedMeshRenderers;
        //虚影透明度
        [Range(0.1f,0.3f)]
        private float Intension = 0.25f;
        private Shader ghostShader;

        //渲染层级
        private int renderQueue = 3000;

        /// <summary>
        /// 虚影节点
        /// </summary>
        private Transform _traShadowNode;

        private ShadowType shadowType = ShadowType.Auto;

        private string shaderName;
        /// <summary>
        /// 模型节点
        /// </summary>
        private Transform traModelNode;

        private Color color = Color.yellow;

        private Transform node;
        private Highlighter highlighter;

        public void Init(Transform node,Transform traShadowNode,Color color,float intension = 0.25f,int renderQueue = 3000,ShadowType shadowType = ShadowType.Auto,string shaderName = "Legacy Shaders/Transparent/Diffuse")
        {
            Intension=intension;
            this.renderQueue=renderQueue;
            this.shadowType=shadowType;
            this.color=color;
            this.shaderName=shaderName;
            traModelNode=traShadowNode;
            ghostShader = Shader.Find(shaderName);
            this.node =node;
        }

        /// <summary>
        /// 初始化虚影,当虚影对象发生变化时，需要重新调用该方法
        /// </summary>

        public void Destroy()
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
            switch (shadowType)
            {
                case ShadowType.Auto:
                    CreateGhost(modelNode);
                    break;
                case ShadowType.Manual:
                    _traShadowNode=traModelNode;
                    var renders = _traShadowNode.GetComponentsInChildren<Renderer>();
                    foreach (var item in renders)
                    {
                        SetMaterial(item.gameObject);
                    }
                    highlighter=  _traShadowNode.gameObject.AddComponent<Highlighter>();
                    highlighter.ConstantOnImmediate(color);
                    break;
                default:
                    break;
            }
            //创建虚影模型
        }

        public void DestroyGhostModels()
        {
            if (_traShadowNode != null)
                Object.Destroy(_traShadowNode.gameObject);
        }

        /// <summary>
        /// 创建虚影节点
        /// </summary>
        private void CreateGhostParent()
        {
            if (_traShadowNode == null)
            {
                _traShadowNode = new GameObject("Ghost").transform;
                _traShadowNode.SetParent(this.node);
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


            _traShadowNode = CreateShadowObject(null,modelNode.transform).transform;

            _traShadowNode.localPosition = modelNode.localPosition;
            _traShadowNode.localRotation = modelNode.localRotation;
            _traShadowNode.localScale = modelNode.localScale;

            //获取到模型的所有节点
            //然后生成Null的
            CreateNode(modelNode,_traShadowNode);

            //添加虚影高亮组件
            highlighter = _traShadowNode.gameObject.AddComponent<Highlighter>();
            highlighter.ConstantOnImmediate(color);
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
            //   Material material = new Material(ghostShader);
            if (meshFilter != null && meshRenderer != null)
            {
                Mesh meshF = new Mesh();
                meshF = meshFilter.sharedMesh;

                go = CreateMesh(parent,meshF,meshFilter.transform,child.name);//meshRenderer.sharedMaterials,child.name);
            }
            else if (skinnedMesh != null)
            {
                Mesh meshS = new Mesh();
                meshS = skinnedMesh.sharedMesh;
                go = CreateMesh(parent,meshS,skinnedMesh.transform,child.name);// skinnedMesh.materials,child.name);
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
        private void CreateNode(Transform firstNode,Transform lastNode)
        {
            for (int i = 0; i < firstNode.childCount; i++)
            {
                var child = firstNode.GetChild(i);
                if (child.name.Equals("distanceParent")||child.name.Equals("Effect")) continue;
                var go = CreateShadowObject(lastNode,child);

                CreateNode(child,go.transform);
            }
        }

        /// <summary>
        /// 创建mesh改shader
        /// </summary>
        /// <param passiveName="mesh"></param>
        /// <param passiveName="tra"></param>
        /// <param passiveName="materials"></param>
        /// <param passiveName="name"></param>
        private GameObject CreateMesh(Transform parent,Mesh mesh,Transform tra,string name = "")// Material[] materials,string name = "")
        {
            GameObject go = new GameObject(name);

            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            MeshRenderer meshRen = go.AddComponent<MeshRenderer>();
            SetMaterial(go);

            if (parent != null)
                go.transform.SetParent(parent);

            go.transform.localScale = tra.transform.localScale;
            go.transform.localPosition = tra.transform.localPosition;
            go.transform.localRotation = tra.transform.localRotation;

            return go;
        }

        private void SetMaterial(GameObject go)
        {

            Renderer meshRen = go.GetComponent<MeshRenderer>();
            if (meshRen==null)
                meshRen= go.GetComponent<SkinnedMeshRenderer>();
            //设置材质
            Material material = new Material(ghostShader)
            {
                renderQueue=renderQueue,
            };
            material.SetColor("_Color",new Color(1f,1f,1f,Intension));
            // meshRen.materials = materials;
            meshRen.material=material;
        }

        /// <summary>
        /// 打开虚影
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="position"></param>
        /// <param name="rotate"></param>
        public void OpenGhost(Transform targetPos,Vector3 position = default(Vector3),Vector3 scale = default(Vector3),Quaternion rotate = default(Quaternion),bool isLocal = true)
        {
            if (_traShadowNode==null)      //虚影缓存为空时新建虚影
            {
                CreateGhostModels(targetPos);
            }
            _traShadowNode.parent = node;
            if (isLocal)
            {

                _traShadowNode.localPosition = position;
                _traShadowNode.localRotation = rotate;
            }
            else
            {
                _traShadowNode.position = position;
                _traShadowNode.rotation = rotate;
            }
            //_traShadowNode.localScale = scale;
            if (_traShadowNode.gameObject.activeSelf == false)
                _traShadowNode.gameObject.SetActive(true);
            highlighter?.ConstantOnImmediate(color);
        }

        /// <summary>
        /// 关闭虚影
        /// </summary>
        public void CloseGhost()
        {
            if (_traShadowNode!=null)
            {
                highlighter?.ConstantOffImmediate();
                if (_traShadowNode.gameObject.activeSelf)
                    _traShadowNode.gameObject.SetActive(false);

                _traShadowNode.SetParent(this.node);
                _traShadowNode.transform.localPosition = Vector3.zero;
                _traShadowNode.transform.localRotation = Quaternion.identity;
                _traShadowNode.transform.localScale = Vector3.one;
            }

        }
    }
}

