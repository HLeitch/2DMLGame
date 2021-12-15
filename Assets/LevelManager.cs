using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    public GameObject startFlag;
    [HideInInspector]
    public Vector3 levelStart { get { return startFlag.transform.position; } }

    [SerializeField]
    public GameObject endFlag;
    [HideInInspector]
    public Vector3 levelEnd { get { return endFlag.transform.position; } }

    [SerializeField]
    public PlayerController player;
    [SerializeField]
    private GameObject playerPrefab;

    void Start()
    {
        if (player == null)
        {
            if(TryGetComponent<PlayerController>(out player))
            {
            }
            else
            {
                player = Instantiate(playerPrefab, levelStart, Quaternion.identity).GetComponent<PlayerController>();
            }
        }
        player.transform.position = levelStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
