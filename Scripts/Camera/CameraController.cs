using UnityEngine;
using Cinemachine;

/// <summary> Controls the camera settings. The camera is controlled by Cinemachine so this is just for basic settings </summary>
public class CameraController : MonoBehaviour
{
    /// <summary> If the player can rotate the camera. </summary>
    public bool canRotate = true;
    /// <summary> Reference of the dialgue camera. </summary>
    public CinemachineVirtualCamera dialogueCam;

    /// <summary> Reference of the player camera. </summary>
    CinemachineFreeLook playerCam;
    /// <summary> The top rig of the player camera. </summary>
    CinemachineBasicMultiChannelPerlin topRig;
    /// <summary> The middle rig of the player camera. </summary>
    CinemachineBasicMultiChannelPerlin middleRig;
    /// <summary> The bottom rig og the player camera. </summary>
    CinemachineBasicMultiChannelPerlin bottomRig;

    private void Start()
    {
        playerCam = FindObjectOfType<CinemachineFreeLook>();
        topRig = playerCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        middleRig = playerCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        bottomRig = playerCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        //If the player can rotate the camera then set the input Names for the Axises to the Mouse Axis. Otherwise set them to null
        if(canRotate)
        {
            playerCam.m_YAxis.m_InputAxisName = "Mouse Y";
            playerCam.m_XAxis.m_InputAxisName = "Mouse X";
        }
        else
        {
            playerCam.m_YAxis.m_InputAxisName = null;
            playerCam.m_XAxis.m_InputAxisName = null;
        }
    }

    /// <summary> Swaps the dialogue and the player cameras. </summary>
    void SwapCam()
    {
        dialogueCam.enabled = playerCam.enabled;
        playerCam.enabled = dialogueCam.enabled;
    }

    #region SetNoise

    /// <summary>
    /// Set the Noise Amplitude and Frequency Gain for all Rigs by two float.
    /// </summary>
    /// <param name="AmplitudeGain">The Amplitude Gain for all Rigs.</param>
    /// <param name="FrequencyGain">The Frequency Gain for all Rigs.</param>
    public void SetNoise(float amplitudeGain = 0f, float frequencyGain = 0f)
    {
        topRig.m_AmplitudeGain = amplitudeGain;
        topRig.m_FrequencyGain = frequencyGain;
        middleRig.m_AmplitudeGain = amplitudeGain;
        middleRig.m_FrequencyGain = frequencyGain;
        bottomRig.m_AmplitudeGain = amplitudeGain;
        bottomRig.m_FrequencyGain = frequencyGain;
    }

    /// <summary>
    /// Set the Noise Settings for each Rig.
    /// </summary>
    /// <param name="Top Rig">The Noise settings Oject for the Top Rig.</param>
    /// <param name="Middle Rig">The Noise settings Object for the Middle Rig.</param>
    /// <param name="Bottom Rig">The Noise settings Object for the Bottom Rig.</param>
    public void SetNoise(NoiseSettings _topRig, NoiseSettings _middleRig, NoiseSettings _bottomRig)
    {
        topRig.m_NoiseProfile = _topRig;
        middleRig.m_NoiseProfile = _middleRig;
        bottomRig.m_NoiseProfile = _bottomRig;
    }

    /// <summary>
    /// Set the Noise Settings, Amplitude Gain and Frequency Gain for each Rig.
    /// </summary>
    /// <param name="amplitudeGain"></param>
    /// <param name="frequencyGain"></param>
    /// <param name="_topRig"></param>
    /// <param name="_middleRig"></param>
    /// <param name="_bottomRig"></param>
    public void SetNoise(float amplitudeGain, float frequencyGain, NoiseSettings _topRig, NoiseSettings _middleRig, NoiseSettings _bottomRig)
    {
        topRig.m_NoiseProfile = _topRig;
        middleRig.m_NoiseProfile = _middleRig;
        bottomRig.m_NoiseProfile = _bottomRig;
        topRig.m_AmplitudeGain = amplitudeGain;
        topRig.m_FrequencyGain = frequencyGain;
        middleRig.m_AmplitudeGain = amplitudeGain;
        middleRig.m_FrequencyGain = frequencyGain;
        bottomRig.m_AmplitudeGain = amplitudeGain;
        bottomRig.m_FrequencyGain = frequencyGain;
    }

    #endregion
}
