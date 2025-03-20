using UnityEngine;

public class BoardManager : BaseSingleton<BoardManager>
{
    [SerializeField] private GameObject _fullAllHardLego, _fullAllRoundedLego, _fullOneRoundedLego, _fullThreeRoundedLego, _fullTwoParallelRoundedLego, _fullTwoRoundedLego,
        _halfAllHardLego, _halfAllRoundedLego, _halfRightRoundedLego, _halfUpRoundedLego, _fullScrew, _halfScrew;

    [SerializeField] private Material[] _materials;
    [SerializeField] private Material _projectionMaterial;

    public GameObject FullAllRoundedLego { get => _fullAllRoundedLego; }
    public GameObject FullAllHardLego { get => _fullAllHardLego; }
    public GameObject FullOneRoundedLego { get => _fullOneRoundedLego; }
    public GameObject FullThreeRoundedLego { get => _fullThreeRoundedLego; }
    public GameObject FullTwoRoundedLego { get => _fullTwoRoundedLego; }
    public GameObject FullTwoParallelRoundedLego { get => _fullTwoParallelRoundedLego; }
    public GameObject HalfAllHardLego { get => _halfAllHardLego; }
    public GameObject HalfAllRoundedLego { get => _halfAllRoundedLego; }
    public GameObject HalfUpRoundedLego { get => _halfUpRoundedLego; }
    public GameObject HalfRightRoundedLego { get => _halfRightRoundedLego; }
    public GameObject FullScrew { get => _fullScrew; }
    public GameObject HalfScrew { get => _halfScrew; }

    public Material[] Materials {  get => _materials; }
    public Material ProjectionMaterial {  get => _projectionMaterial; }

    private static int _touchable = 0;

    public void LockTouchable()
    {
        _touchable++;
    }

    public void UnlockTouchable()
    {
        _touchable--;
    }

    public bool IsTouchable()
    {
        if (_touchable == 0) { return true; }
        else { return false; }
    }
}
