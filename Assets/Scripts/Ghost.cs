using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
   public Piece trackingPiece;
   public Tile tile;
   public Board board;
   public Tilemap tilemap {get; private set;}
   public Vector3Int[] cells{get; private set;}
   public Vector3Int position {get; private set;}

   private void Awake() {
    this.tilemap = GetComponentInChildren<Tilemap>();
    this.cells = new Vector3Int[4];
   }

   private void LateUpdate() {
        Clear();
        Copy();
        Drop();
        Set();
   }

    private void Clear(){
        for(int i = 0; i < this.cells.Length; i++){
            Vector3Int tilePos = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePos, null);
        }
    }

    private void Copy(){
        for(int i = 0; i < this.cells.Length; i++){
            this.cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop(){

        Vector3Int position = this.trackingPiece.position;
        int current = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Clear(this.trackingPiece);

        for(int row = current; row >= bottom; row--){
            position.y = row;
            if(this.board.IsValidPosition(this.trackingPiece, position)){
                this.position = position;
            }
            else{
                break;
            }
        }

        this.board.Set(this.trackingPiece);

    }

    public void Set(){
        for(int i = 0; i < this.cells.Length; i++){
            Vector3Int tilePos = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePos, this.tile);
        }
    }
   
}
