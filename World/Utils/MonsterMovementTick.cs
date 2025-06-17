using World.GameWorld;

public class MonsterMovementTick
{
    private readonly MapInstance _mapInstance;
    private readonly CancellationToken _token;

    public MonsterMovementTick(MapInstance mapInstance, CancellationToken token)
    {
        _mapInstance = mapInstance;
        _token = token;
    }

    public async Task StartAsync()
    {
        while (!_token.IsCancellationRequested)
        {
            var movingMonsters = _mapInstance.MonsterEntities.Where(m => m.IsMoving).ToList();

            foreach (var monster in movingMonsters)
            {
                await monster.Move();
            }

            if (movingMonsters.Count > 0)
            {
                var minSpeed = movingMonsters.Min(m => m.NpcInfo.Speed);
                var delay = Math.Round(minSpeed * 1.5f) * 10;
                await Task.Delay(minSpeed * 177, _token);
            }
            else
            {
                await Task.Delay(2500, _token); // Evita un loop vacío apretando CPU
            }
        }
    }

}
