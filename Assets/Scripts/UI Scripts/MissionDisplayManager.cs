using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDisplayManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MissionDisplay _missionDisplayUIPrefab;
    [SerializeField] private GameObject _missionDisplayWindowParent;
    [SerializeField] private bool _slowlyGenerateTextAtStart = true;
    [SerializeField] private float _delayPerCharacter = 0.05f;

    private List<MissionDisplay> _missionDisplayList;
    //private bool _charactersAlreadyGenerated = false;     // True if coroutine already ran, meaning it should only run once.
    //private bool _descriptionGenerationEnded = false;
    private Coroutine _charGeneratorCoroutine;

    public void Initialize(List<MissionObjective> objectives)
    {
        _missionDisplayList = new List<MissionDisplay>(objectives.Count);
        for (int i = 0; i < objectives.Count; i++)
        {
            MissionDisplay currentDisplay = Instantiate(_missionDisplayUIPrefab, _missionDisplayWindowParent.transform);
            _missionDisplayList.Add(currentDisplay);
            if (_slowlyGenerateTextAtStart) StartCoroutine(GenerateTitle(objectives[i].MissionTitle, i));
            else currentDisplay.MissionTitle.text = objectives[i].MissionTitle;
            UpdateMissionDisplay(objectives[i], i);
        }
    }

    public void UpdateMissionDisplay(MissionObjective objective, int missionIndex)
    {
        // If objective's mission isn't singular (e.g. defeat 5 enemies), add text to match this.
        string newDescription = objective.MissionDescription;
        if (objective.MaxProgress > 1)
        {
            newDescription += $" ({objective.CurrentProgress}/{objective.MaxProgress})";
        }

        if (_slowlyGenerateTextAtStart)
        {
            //if (!_charactersAlreadyGenerated)
            //{
                _charGeneratorCoroutine = StartCoroutine(GenerateDescription(newDescription, missionIndex));
            //}
            /*else
            {
                if (!_descriptionGenerationEnded) StopCoroutine(_charGeneratorCoroutine);
                _missionDisplayList[missionIndex].MissionDescription.text = newDescription;
            }*/
        }
        else
        {
            _missionDisplayList[missionIndex].MissionDescription.text = newDescription;
        }
    }

    // Slowly types out the characters of the text one at a time.
    private IEnumerator GenerateTitle(string title, int missionIndex)
    {
        char[] characters = title.ToCharArray();
        string newDescription = string.Empty;
        for (int i = 0; i < characters.Length; i++)
        {
            newDescription += characters[i];
            _missionDisplayList[missionIndex].MissionTitle.text = newDescription;
            if (char.IsWhiteSpace(characters[i])) continue;
            yield return new WaitForSeconds(_delayPerCharacter);
        }
        yield return null;
    }

    // Slowly types out the characters of the text one at a time.
    private IEnumerator GenerateDescription(string description, int missionIndex)
    {
        //_charactersAlreadyGenerated = true;
        char[] characters = description.ToCharArray();
        string newDescription = string.Empty;
        for (int i = 0; i < characters.Length; i++)
        {
            newDescription += characters[i];
            _missionDisplayList[missionIndex].MissionDescription.text = newDescription;
            if (char.IsWhiteSpace(characters[i])) continue;
            yield return new WaitForSeconds(_delayPerCharacter);
        }
        //_descriptionGenerationEnded = true;
        yield return null;
    }
}
