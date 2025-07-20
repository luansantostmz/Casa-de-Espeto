using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class QTESystem : MonoBehaviour
{
    [SerializeField] Transform _center;
    [SerializeField] float _radius;
    [SerializeField] Button _hammerButtonPrefab;
    [SerializeField] int _hitsCount;
    [SerializeField] float _qteDuration;
    [SerializeField] Image _performanceBar;
    [SerializeField] int _hammerPoints;
    [SerializeField] float _currentScore;
    [SerializeField] float _scoreDecreasePower;
    [SerializeField] int _maxScore;

    float _timer;
    bool _inProgress;
    Button _currentButton;

    [Button]
    public void StartQTE()
    {
        _timer = _qteDuration;
        _inProgress = true;
        SpawnButton();
    }

    void StopQTE()
    {
        _inProgress = false;
        Destroy(_currentButton.gameObject);
    }

    public void Hammer()
    {
        _currentScore += _hammerPoints;
        if (_currentScore > _maxScore)
            _currentScore = _maxScore;

        _performanceBar.fillAmount = (float)_currentScore / (float)_maxScore;

        SpawnButton();
    }

    void SpawnButton()
    {
        if (_currentButton)
            Destroy(_currentButton.gameObject);

        _currentButton = Instantiate(_hammerButtonPrefab, _center);
        Vector2 pos = Random.insideUnitCircle.normalized * _radius;
        _currentButton.transform.localPosition = pos;

        _currentButton.onClick.AddListener(Hammer);
        _currentButton.onClick.AddListener(() =>
        {
            Destroy(_currentButton.gameObject);
        });
    }

    public void Update()
    {
        if (!_inProgress)
            return;

        _timer -= Time.deltaTime;
        _currentScore -= Time.deltaTime * _scoreDecreasePower;
        _performanceBar.fillAmount = (float)_currentScore / (float)_maxScore;

        if (_timer <= 0)
        {
            StopQTE();
        }
    }
}