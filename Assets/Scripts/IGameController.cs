using Unity;
using System.Collections;
using System.Collections.Generic;

public interface IGameController
{
    IEnumerator Start();
    IEnumerator _AutoPlay();
    void ToggleAutoPlay();
    void StopAutoPlay(bool force = false);
    void SetSelectedItem(LearningPartRef partRef);
    void Update();
    void NextPage();
    void PrevPage();
    void PlayCurrentPage(int pgIndex);
    void SetLabelOnOrOff(bool isOn);
    void DisableTargetObjects(bool isOn);
    bool IsItemSelected { get; }
    LearningPartRef SelectedItem { get; }
}