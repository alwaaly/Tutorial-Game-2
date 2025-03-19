using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour {
    public SignVlaue Value;
    [SerializeField] TextMeshProUGUI[] textUGUIs;
    [SerializeField] Image[] images;
    private void OnValidate() {
        textUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
        images = GetComponentsInChildren<Image>();
    }
    private void Update() {
        transform.position += Vector3.forward * -1 * Time.deltaTime * LevelManager.Instance.LevelSpeed;
        if (transform.position.z < -10) gameObject.SetActive(false);
    }
    public void GenerateNumber() {
        Value = new(textUGUIs, images);
    }
}
public class SignVlaue {
    public Queue<int> SignNumbers;
    public Queue<Color> SignColor;
    public Queue<int> SignMath;
    public SignVlaue(TextMeshProUGUI[] textUGUIs, Image[] images) {
        SignNumbers = new(4);
        SignColor = new(4);
        SignMath = new(4);


        int number;
        Color color;
        RandomMath math =new();

        for (int i = 0; i < 4; i++) {
            number = LevelManager.Instance.GetRandomNumberExcept(SignNumbers.ToList());
            color = LevelManager.Instance.GetRandomColorExcept(SignColor.ToList());

            textUGUIs[i].text = "";
            images[i].color = Color.black;

            if (LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Color &&
                LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Math &&
                LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.MathAndColor)
                textUGUIs[i].text = number.ToString();
            if (LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Number &&
                LevelManager.Instance.TheStateOFRandomness != LevelManager.RandomState.Math)
                images[i].color = color;
            if(LevelManager.Instance.TheStateOFRandomness == LevelManager.RandomState.Math ||
                LevelManager.Instance.TheStateOFRandomness == LevelManager.RandomState.MathAndColor) {
                math = LevelManager.Instance.GetRandomMath();
                switch (math.Operation) {
                    case RandomMath.MathOperation.Add:
                        textUGUIs[i].text = math.FirstNumber + "\n" + "+\n" + math.SecoundNumber;
                        break;

                    case RandomMath.MathOperation.Subtract:
                        textUGUIs[i].text = math.FirstNumber + "\n" + "-\n" + math.SecoundNumber;
                        break;
                }
            }

            SignNumbers.Enqueue(number);
            SignColor.Enqueue(color);
            SignMath.Enqueue(math.Result);
        }
    }
}
