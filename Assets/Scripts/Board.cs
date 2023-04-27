using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominos;
    public Piece activePiece{get; private set;}
    public  Tilemap tilemap {get; private set;}

    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public int score = 0;

    public Text scoreText;
    public RectInt Bounds{
        get{
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }
    private void Awake(){

        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for(int i = 0; i < this.tetrominos.Length; i++){
            this.tetrominos[i].initialize();
        }
    }

    private void Start() {
        SpawnPiece();
    }

    public void SpawnPiece(){
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = this.tetrominos[random]; 

        this.activePiece.Initialize(this, spawnPosition, data);
        
        if(IsValidPosition(this.activePiece, this.spawnPosition)){
            Set(this.activePiece);
        }
        else{
            GameOver();
        }

    }

    private void GameOver(){
        //Add more features here.
        this.tilemap.ClearAllTiles();
        this.score = 0;
        this.activePiece.levelText.text = "1";
        this.scoreText.text = this.score.ToString();
    }
    public void Set(Piece piece){
        for(int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, piece.data.tile);
        }
    }

    public void clearLines(){

        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        while(row < bounds.yMax){
            if(isLineFull(row)){
                LineClear(row);
                score++;
            }
            else{
                row++;
            }
        }
        this.scoreText.text = score.ToString();
    }
    private bool isLineFull(int row){

        RectInt bounds = this.Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++){

            Vector3Int position = new Vector3Int(col, row, 0);

            if(!this.tilemap.HasTile(position)){
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row){
        RectInt bounds = this.Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++){
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }

        while(row < Bounds.yMax){
            for(int col = bounds.xMin; col < bounds.xMax; col++){
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }
            
            row++;
        }
    }

    public void Clear(Piece piece){
        for(int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePos = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePos, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position){

        RectInt bounds = this.Bounds;
        for(int i = 0; i < piece.cells.Length; i++){
            Vector3Int tilePosition = piece.cells[i] + position;

            if(!bounds.Contains((Vector2Int)tilePosition)){
                return false;
            }
            if(this.tilemap.HasTile(tilePosition)){
                return false;
            }
            
        }

        return true;
    }
}
