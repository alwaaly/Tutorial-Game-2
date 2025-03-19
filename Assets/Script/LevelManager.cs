using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField] Material floorMaterial;
    [SerializeField] Wall[] AllWallsType;
    [SerializeField] Material[] allWallsMaterial;
    public Sign sign;
    List<int> randomNumbers;
    [SerializeField] List<Color> randomColors;
    BakeWall[] bakeWalls;
    public float LevelSpeed = 10;
    public static LevelManager Instance;
    public MainCharctere mainCharctere;

    public enum RandomState { Number ,Color , NumberAndColor , Math , MathAndColor }
    public RandomState TheStateOFRandomness;
    public AudioSource DooraudioSource;
    private void Awake() {
        Instance = this;
        randomNumbers = new() {
            0,1,2,3,4,5,6,7,8,9
        };
        bakeWalls = new BakeWall[AllWallsType.Length];
        for (int i = 0; i < AllWallsType.Length; i++) {
            bakeWalls[i] = new(4, AllWallsType[i]);
        }
        TheStateOFRandomness = (RandomState)UnityEngine.Random.Range(0, Enum.GetValues(typeof(RandomState)).Length);
        Generate();
        floorMaterial.SetFloat("_movmentSpeed", .15f);

        mainCharctere.OnPlayerDie += MainCharctere_OnPlayerDie;
    }

    private void MainCharctere_OnPlayerDie() {
        floorMaterial.SetFloat("_movmentSpeed", 0);
        LevelSpeed = 0;
    }

    public void Generate() {
        for (int i = 0; i < 5; i++) {
            if(i == 0) {
                sign.transform.position = new(sign.transform.position.x, sign.transform.position.y, 60);
                sign.gameObject.SetActive(true);
                sign.GenerateNumber();
            }
            else {
                bakeWalls[UnityEngine.Random.Range(0, bakeWalls.Length)].ShowWall(new(0, 0, 60 + (20 * i)), allWallsMaterial[UnityEngine.Random.Range(0, allWallsMaterial.Length)]);
            }
        }
    }
    public int GetRandomNumber() {
        return UnityEngine.Random.Range(0, 10);
    }
    public int GetRandomNumberExcept(List<int> exceptNumbers) {
        List<int> randomNumberCopy = new(randomNumbers);
        for (int i = 0; i < exceptNumbers.Count; i++) {
            randomNumberCopy.Remove(exceptNumbers[i]);
        }
        return randomNumberCopy[UnityEngine.Random.Range(0, randomNumberCopy.Count)];
    }
    public int GetNearestRandomToNumber(int number) {
        if (number == 0 || number == 9) {
            if (number == 0) return UnityEngine.Random.Range(0, 3);
            else return UnityEngine.Random.Range(7, 10);
        }
        else return UnityEngine.Random.Range(number - 1, number + 2);
    }
    public Color GetRandomColorExcept(List<Color> exceptNumbers) {
        List<Color> randomColorCopy = new(randomColors);
        for (int i = 0; i < exceptNumbers.Count; i++) {
            randomColorCopy.Remove(exceptNumbers[i]);
        }
        return randomColorCopy[UnityEngine.Random.Range(0, randomColorCopy.Count)];
    }
    public Color GetNearestRandomToColor(Color targetColor) {
        float nerrestDistance = 100000000;
        Color nerrestColor = Color.black;
        float distance = 0;
        for (int i = 0; i < randomColors.Count; i++) {
            distance = CalculateColorDistance(targetColor, randomColors[i]);
            if (distance == 0) continue;
            if (distance < nerrestDistance) {
                nerrestDistance = distance;
                nerrestColor = randomColors[i];
            }
        }
        if (UnityEngine.Random.Range(0, 2) == 0) return targetColor;
        else return nerrestColor;
    }
    public Color GetNearestColorTo(Color targetColor) {
        float nerrestDistance = 100000000;
        Color nerrestColor = Color.black;
        float distance = 0;
        for (int i = 0; i < randomColors.Count; i++) {
            distance = CalculateColorDistance(targetColor, randomColors[i]);
            if (distance == 0) continue;
            if (distance < nerrestDistance) {
                nerrestDistance = distance;
                nerrestColor = randomColors[i];
            }
        }
        return nerrestColor;
    }
    private float CalculateColorDistance(Color color1, Color color2) {
        return Vector3.Distance(new Vector3(color1.r, color1.g, color1.b), new Vector3(color2.r, color2.g, color2.b));
    }
    public RandomMath GetRandomMath() {
        RandomMath math = new();
        math.FirstNumber = UnityEngine.Random.Range(0, 10);
        math.Operation = (RandomMath.MathOperation)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(RandomMath.MathOperation)).Length);
        switch (math.Operation) {
            case RandomMath.MathOperation.Add:
                math.SecoundNumber = UnityEngine.Random.Range(0, 10 - math.FirstNumber);
                math.Result = math.FirstNumber + math.SecoundNumber;
                break;
            case RandomMath.MathOperation.Subtract:
                math.SecoundNumber = UnityEngine.Random.Range(0, math.FirstNumber + 1);
                math.Result = math.FirstNumber - math.SecoundNumber;
                break;
        }

        return math;
    }
    class BakeWall {
        Queue<Wall> walls;
        Wall tmpWall;
        public BakeWall(int count,Wall type) {
            walls = new(count);
            for (int i = 0; i < count; i++) {
                tmpWall = Instantiate(type);
                walls.Enqueue(tmpWall);
                tmpWall.gameObject.SetActive(false);
            }
        }
        public void ShowWall(Vector3 pos,Material material) {
            tmpWall = walls.Dequeue();
            tmpWall.gameObject.SetActive(true);
            tmpWall.transform.position = pos;
            tmpWall.SetDoor();
            tmpWall.MeshRenderer.material = material;
            walls.Enqueue(tmpWall);
        }
    }
}
public struct RandomMath {
    public int FirstNumber;
    public int SecoundNumber;
    public int Result;
    public MathOperation Operation;
    public enum MathOperation { Add, Subtract }
}