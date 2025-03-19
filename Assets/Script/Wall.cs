using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
    [SerializeField] Door[] doors;
    public MeshRenderer MeshRenderer;

    private void OnValidate() {
        doors = GetComponentsInChildren<Door>();
        if (MeshRenderer == null) MeshRenderer = GetComponent<MeshRenderer>();
    }
    private void Update() {
        transform.position += Vector3.forward * -1 * Time.deltaTime * LevelManager.Instance.LevelSpeed;
        if (transform.position.z < -10) gameObject.SetActive(false);
    }
    public void SetDoor() {
        int selectedDoor = Random.Range(0, doors.Length);
        List<int> doorsNumber = new();
        List<Color> doorsColor = new();

        int tmpNumber;
        Color tmpColor;


        int CorrectNumber;
        Color CorrectColor;

        CorrectNumber = 0;

        if (LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Math &&
            LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.MathAndColor)
            CorrectNumber = LevelManager.Instance.sign.Value.SignNumbers.Dequeue();
        else CorrectNumber = LevelManager.Instance.sign.Value.SignMath.Dequeue();

        CorrectColor = LevelManager.Instance.sign.Value.SignColor.Dequeue();

        doorsNumber.Add(CorrectNumber);
        doorsColor.Add(CorrectColor);

        for (int i = 0; i < doors.Length; i++) {
            doors[i].textUGUI.text = "";
            doors[i].Image.color = Color.black;

            if (i == selectedDoor) {
                doors[i].IsOpen = true;

                if (LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Color)
                    doors[i].textUGUI.text = CorrectNumber.ToString();
                if (LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Number &&
                    LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Math)
                    doors[i].Image.color = CorrectColor;
            }
            else {

                if(LevelManager.Instance.TheStateOFRandomness == LevelManager.RandomState.NumberAndColor ||
                    LevelManager.Instance.TheStateOFRandomness == LevelManager.RandomState.MathAndColor)
                    tmpNumber = LevelManager.Instance.GetNearestRandomToNumber(CorrectNumber);

                else tmpNumber = LevelManager.Instance.GetRandomNumberExcept(doorsNumber);

                tmpColor = LevelManager.Instance.GetRandomColorExcept(doorsColor);

                doors[i].IsOpen = false;

                doorsNumber.Add(tmpNumber);
                doorsColor.Add(tmpColor);

                if (LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Color)
                    doors[i].textUGUI.text = tmpNumber.ToString();

                if (LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Number &&
                    LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Math) {
                    if(LevelManager.Instance.TheStateOFRandomness == LevelManager.RandomState.Color)
                        doors[i].Image.color = tmpColor;
                    else {
                        if (tmpNumber == CorrectNumber) doors[i].Image.color = LevelManager.Instance.GetNearestColorTo(CorrectColor);
                        else doors[i].Image.color = LevelManager.Instance.GetNearestRandomToColor(CorrectColor);
                    }
                }
            }
        }
    }
}
