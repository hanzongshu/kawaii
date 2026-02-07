
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
        {
           [SerializeField]
            Rigidbody2D _rig;
            void Start()
            {
            _rig = GetComponent<Rigidbody2D>();
            }

            // Update is called once per frame
            void Update()
            {

            }
        }


