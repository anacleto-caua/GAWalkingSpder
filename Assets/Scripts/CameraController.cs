using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Experimental.Video;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

/**
 * Esta classe é responsável por controlar a câmera do usuário.
 * Andar com WASD, olhar com o mouse e subir e descer com Q e E.
 * 
 */
public class CameraController : MonoBehaviour
{
    #region movementVars
    private CharacterController characterController;

    private float cameraSpeed = 5f;
    private float cameraAcelleration = 2.5f;

    private float upDownAxisStrengh = 2.0f;
    #endregion movementVars
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        #region camera3dMovement
        float upDownAxis = 0;
        if (Input.GetKey(KeyCode.Q)){
            upDownAxis -= upDownAxisStrengh;
        }
        if (Input.GetKey(KeyCode.E)){
            upDownAxis += upDownAxisStrengh;
        }
        
        Vector3 forward = (characterController.transform.forward * Input.GetAxis("Vertical")) * cameraSpeed * Time.deltaTime;
        Vector3 sideways = (characterController.transform.right * Input.GetAxis("Horizontal")) * cameraSpeed * Time.deltaTime;
        Vector3 vertical = characterController.transform.up * upDownAxis * cameraSpeed *Time.deltaTime;

        if(Input.GetKey(KeyCode.LeftShift)) {
            vertical *= cameraAcelleration;
            forward *= cameraAcelleration;
            sideways *= cameraAcelleration;
        }

        characterController.Move(vertical);
        characterController.Move(forward);
        characterController.Move(sideways);
        #endregion camera3dMovement

        #region cameraLooking
        if (Input.GetMouseButton(1))
        {
            Vector3 lookForward = characterController.transform.eulerAngles;
            lookForward.y += Input.GetAxis("Mouse X");
            lookForward.x += -Input.GetAxis("Mouse Y");

            characterController.transform.eulerAngles = lookForward;
        }
        #endregion cameraLooking
    }
}
