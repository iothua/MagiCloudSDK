using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.NetWorks
{
    public class UnityControl : MonoBehaviour
    {
        private ControlTerminal controlTerminal;
        private ExperimentEventControl eventControl;

        public Text textSocket;

        private void Awake()
        {
            textSocket.text = "Control：";

            controlTerminal = new ControlTerminal();
            controlTerminal.Start("127.0.1", 8888);

            eventControl = new ExperimentEventControl(controlTerminal,(log)=> {
                textSocket.text += log + "    "; 
            });
            
        }

        private void Update()
        {
            controlTerminal.Update();
        }

        private void OnDestroy()
        {
            controlTerminal.Close();
            //threadBreak = true;
        }
    }
}
