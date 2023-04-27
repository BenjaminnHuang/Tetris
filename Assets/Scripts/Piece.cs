using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public Board board{get; private set;}
    public TetrominoData data{get; private set;}
    public Vector3Int[] cells{get; private set;}
    public Vector3Int position{get; private set;}
    
    public int rotationIndex{get; private set;}
    
    public float stepDelay = 1.0f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    private int level = 1;
    public Text levelText;
    public void Initialize(Board board, Vector3Int position, TetrominoData data){

        level = 1;
        setSpeed(board.score);
        
        this.board = board;
        this.position = position;
        this.data = data;
        rotationIndex = 0;
        stepTime = Time.time + this.stepDelay;
        lockTime = 0.0f;

        this.levelText.text = level.ToString();

        if(this.cells == null){
            this.cells = new Vector3Int[data.cells.Length];
        }

        for(int i = 0; i < data.cells.Length; i++){
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void setSpeed(int score){
        if(score >= 10){
            this.stepDelay = 0.7f;
            level = 2;
        }
        else if(score >= 20){
            this.stepDelay = 0.5f;
            level = 3;
        }
        else if(score >= 30){
            this.stepDelay = 0.3f;
            level = 4;
        }
        else if(score < 10){
            this.stepDelay = 1.0f;
            level = 1;
        }
    }
    private void Update() {

        this.board.Clear(this);

        this.lockTime += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Q)){
            Rotate(1);
        }
        else if(Input.GetKeyDown(KeyCode.E)){
            Rotate(-1);
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            Move(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow)){
           Move(Vector2Int.right);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)){
            Move(Vector2Int.down);
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            HardDrop();
        }

        if(Time.time >= this.stepTime){
            Step();
        }
        this.board.Set(this);
    }

    private void Step(){
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        if(this.lockTime >= this.lockDelay){
            Lock();
        }
    }

    private void Lock(){
        this.board.Set(this);
        this.board.clearLines();
        this.board.SpawnPiece();
    }
    private void HardDrop(){

        while(Move(Vector2Int.down)){
            continue;
        }

        Lock();
    }

    private bool Move(Vector2Int translation){
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if(valid){
            this.position = newPosition;
            this.lockTime = 0;
        }

        return valid;
    }

    private bool TestWallKicks(int rotationIndex, int direction){
        int wallKickIndex = GetWallKickIdex(rotationIndex, direction);

        for(int i = 0; i < this.data.wallKicks.GetLength(1); i++){
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            if(Move(translation)){
                return true;
            }
        }        
        return false;
    }

    private int GetWallKickIdex(int rotationIndex, int direction){
        int wallKickIndex = rotationIndex * 2;

        if(direction < 0){
            wallKickIndex--;

        }
        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }
    private void Rotate(int direction){
        int originalIndex = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);
        
        ApplyRotation(direction);

        if(!TestWallKicks(this.rotationIndex, direction)){
            this.rotationIndex = originalIndex;
            ApplyRotation(-direction);
        }
        
    }

    private void ApplyRotation(int direction){
        for(int i = 0; i < this.cells.Length; i++){
            Vector3 cell = this.cells[i];

            int x, y;

            switch(this.data.tetromino){
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt(cell.x * Data.RotationMatrix[0] * direction + cell.y * Data.RotationMatrix[1] * direction);
                    y = Mathf.CeilToInt(cell.x * Data.RotationMatrix[2] * direction + cell.y * Data.RotationMatrix[3] * direction);
                    break;
                default:
                    x = Mathf.RoundToInt(cell.x * Data.RotationMatrix[0] * direction + cell.y * Data.RotationMatrix[1] * direction);
                    y = Mathf.RoundToInt(cell.x * Data.RotationMatrix[2] * direction + cell.y * Data.RotationMatrix[3] * direction);
                    break;
            }

            this.cells[i] = new Vector3Int(x,y);
        }
    }
    private int Wrap(int input, int min, int max){
        if(input < min){
            return max - (min - input) % (max - min);
        }
        else{
            return min + (input - min) % (max - min);
        }
    }
}
