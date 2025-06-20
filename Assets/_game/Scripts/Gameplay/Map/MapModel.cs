using R3;

public class MapModel
{
    public int row;
    public int column;
    public ReactiveProperty<TileEnum>[][] tiles;

    public MapModel(MapData mapData)
    {
        this.row = mapData.row;
        this.column = mapData.column;
        tiles = new ReactiveProperty<TileEnum>[row][];
        for (int i = 0; i < row; i++)
        {
            tiles[i] = new ReactiveProperty<TileEnum>[column];
            for (int j = 0; j < column; j++)
            {
                tiles[i][j] = new ReactiveProperty<TileEnum>((TileEnum)mapData.tiles[i][j]);
            }
        }
    }    
}