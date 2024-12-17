using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Used for the Hat selection logic
public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField]
    private Transform m_HatAnchor;

    private AsyncOperationHandle m_HatLoadingHandle;
    private bool hatHandleCreated = false;

    private ApplyRemoteConfigSettings remoteConfigScript;

    void Start()
    {   
        // Get the instance of ApplyRemoteConfigSettings
        remoteConfigScript = ApplyRemoteConfigSettings.Instance;

        // Call the FetchConfigs() to see if there's any new settings
        remoteConfigScript.FetchConfigs();
        
        //If the condition is met, then a hat has been unlocked
        if(GameManager.s_ActiveHat >= 0)
        {
            //SetHat(string.Format("Hat{0:00}", UnityEngine.Random.Range(0, 4)));

            // Fetch the correct hat variable from the ApplyRemoteConfigSettings instance
            if (ApplyRemoteConfigSettings.Instance.season == "Default")
            {
               //Debug.Log("Formatted String 2 " + string.Format("Hat{0:00}", remoteConfigScript.activeHat));
               Debug.Log($"Setting hat from remote config script {remoteConfigScript.activeHat}");
               SetHat(string.Format("Hat{0:00}", remoteConfigScript.activeHat));
            }

            else if (ApplyRemoteConfigSettings.Instance.season == "Winter")
            {
                Debug.Log("Setting hat from winter season");
                SetHat(string.Format("Hat{0:00}", "04"));
            }

            else if (ApplyRemoteConfigSettings.Instance.season == "Halloween")
            {
                Debug.Log("Setting hat from halloween season");
                SetHat(string.Format("Hat{0:00}", "05"));
            }

            else
            {
                Debug.Log($"Setting hat from local game manager {GameManager.s_ActiveHat}");
                SetHat(string.Format("Hat{0:00}", GameManager.s_ActiveHat));
            }

        }
        else
        {
            Debug.Log("No hat configured");
        }
    }

    public void SetHat(string hatKey)
    {
        // We are using the InstantiateAsync function on the Addressables API, the non-Addressables way 
        // looks something like the following line, however, this version is not Asynchronous
        // GameObject.Instantiate(prefabToInstantiate);

        Debug.Log($"Setting Hat {hatKey}");
        m_HatLoadingHandle = Addressables.InstantiateAsync(hatKey, m_HatAnchor, false);

        m_HatLoadingHandle.Completed += OnHatInstantiated;
        hatHandleCreated = true;
    }

    private void OnDisable()
    {
        if(hatHandleCreated)
            m_HatLoadingHandle.Completed -= OnHatInstantiated;
    }

    private void OnHatInstantiated(AsyncOperationHandle obj)
    {
        // We can check for the status of the InstantiationAsync operation: Failed, Succeeded or None
        if(obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Hat instantiated successfully");
        }
        else
        {
            Debug.LogWarning("Hat instantiation failed");
        }

        if(hatHandleCreated)
        {
            m_HatLoadingHandle.Completed -= OnHatInstantiated;
            hatHandleCreated = false;
        }
    }
}
