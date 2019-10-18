using UnityEngine;
using Cinemachine;

namespace Character.DialogueSystem
{
    /// <summary> Controlls the dialogue camera. </summary>
    public class DialogueCamera : MonoBehaviour
    {
        /// <summary> Array of the character transforms who take part of the dialogue. </summary>
        public Transform[] characterTrans = new Transform[2];

        /// <summary> Reference of the CameraController. </summary>
        private CameraController camControl;
        /// <summary> The dialogue camera. </summary>
        private CinemachineVirtualCamera dialogueCam;
        /// <summary> Reference of the CinemaachineTargetGroup component wich the dialogue camera uses. </summary>
        private CinemachineTargetGroup targetGroup;

        private void Start()
        {
            camControl = FindObjectOfType<CameraController>();
            dialogueCam = camControl.dialogueCam;
            targetGroup = dialogueCam.Follow.GetComponent<CinemachineTargetGroup>();
        }

        #region SetNarrators

        void SetNarrator(Transform[] narrators)
        {
            for(int i = 0; i < narrators.Length; i++)
            {
                var targetNarrator = new CinemachineTargetGroup.Target();
                targetNarrator.target = narrators[i];
                targetNarrator.weight = 1f;
                targetNarrator.radius = 1.5f;
                targetGroup.m_Targets[i] = targetNarrator;
            }
        }

        void SetNarrator(Transform[] narrators, float[] weight)
        {
            for (int i = 0; i < narrators.Length; i++)
            {
                var targetNarrator = new CinemachineTargetGroup.Target();
                targetNarrator.target = narrators[i];
                targetNarrator.weight = weight[i];
                targetNarrator.radius = 1.5f;
                targetGroup.m_Targets[i] = targetNarrator;
            }
        }

        void SetNarrator(Transform[] narrators, float[] weight, float[] radius)
        {
            for (int i = 0; i < narrators.Length; i++)
            {
                var targetNarrator = new CinemachineTargetGroup.Target();
                targetNarrator.target = narrators[i];
                targetNarrator.weight = weight[i];
                targetNarrator.radius = radius[i];
                targetGroup.m_Targets[i] = targetNarrator;
            }
        }

        #endregion

        void LookAtNarrator()
        {

        }
    }
}
