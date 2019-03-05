using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadMainLevel : MonoBehaviour 
{
	private bool levelLoaded = false;
	
	void Update() 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(!levelLoaded && manager && KinectManager.IsKinectInitialized())
		{
			levelLoaded = true;

            SceneManager.LoadScene(1);
			//Application.LoadLevel(1);
		}
	}
	
}
