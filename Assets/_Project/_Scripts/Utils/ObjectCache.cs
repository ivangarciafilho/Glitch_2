using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ObjectCache : MonoBehaviour
{
    public GameObject prefab;
    public int prefabCount = 5;
    protected GameObject[] instances;
    public bool slowlyInstantiation = false;
    public bool prewarmPrefabs = false;
    public bool makeThemMyChild = false;

    protected int nextIndexToReturn = 0;

    public bool respawnWhenNull = false;

    protected virtual void Awake()
    {
        instances = new GameObject[prefabCount];

        if (slowlyInstantiation)
        {
            StartCoroutine(SlowInstantiation());
        }
        else if(!prewarmPrefabs)
        {
            for (int i = 0; i < prefabCount; i++)
            {
                instances[i] = Instantiate(prefab);
                instances[i].SetActive(prewarmPrefabs ? true : false);

                if(makeThemMyChild)
                {
                    instances[i].transform.SetParent(transform);
                    instances[i].transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    protected virtual void Start()
    {
        if (prewarmPrefabs && !slowlyInstantiation)
        {
            for (int i = 0; i < prefabCount; i++)
            {
                instances[i] = Instantiate(prefab);
                instances[i].SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        if(respawnWhenNull)
        {
            for (int i = 0; i < instances.Length; i++)
            {
                if (instances[i] == null)
                {
                    instances[i] = Instantiate(prefab);
                    instances[i].SetActive(false);
                }
            }
        }
    }

    IEnumerator SlowInstantiation()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < prefabCount; i++)
        {
            instances[i] = Instantiate(prefab);
            instances[i].SetActive(false);

            yield return new WaitForSeconds(0.6f);
        }
    }

    private void OnEnable()
    {
        if (prefabCount < 1)
        {
            Debug.Log("Nothing To Instantiate");
            DestroyImmediate(this);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < prefabCount; i++)
        {
        	if(instances[i] != null)
        	{
            	instances[i].SetActive(false);
	        	Destroy(instances[i], 0.5f);
        	}
        }
    }

    public void DisableAll()
    {
        for (int i = 0; i < instances.Length; i++)
            instances[i].SetActive(false);

        nextIndexToReturn = 0;
    }

    public GameObject GetObject()
    {
        if (nextIndexToReturn >= instances.Length) nextIndexToReturn = 0;

        GameObject objToReturn = instances[nextIndexToReturn];
        objToReturn.SetActive(false);

        nextIndexToReturn++;
        return objToReturn;
    }

    public T GetObject<T>()
    {
        if (nextIndexToReturn >= instances.Length) nextIndexToReturn = 0;

        GameObject objToReturn = instances[nextIndexToReturn];
        objToReturn.SetActive(false);

        nextIndexToReturn++;
        return objToReturn.GetComponent<T>();
    }
}