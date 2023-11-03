using Altseed2;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

StreamReader streamReader = new StreamReader("./games.txt");

List<string> games = new();
var line = streamReader.ReadLine();
while(line != null)
{
    games.Add(line);
    line = streamReader.ReadLine();
}

int selectedGameIdx = 0;

List<Texture2D> coverTex = new();

Engine.Initialize("GameLauncher", 720 / 9 * 16, 720);

for(int i = 0; i < games.Count; i++)
    coverTex.Add(Texture2D.LoadStrict("./" + games[i] + "/cover.png"));

SpriteNode coverNode = new SpriteNode();
coverNode.Position = new Vector2F(0, 0);
Engine.AddNode(coverNode);
void setCover(int idx)
{
    coverNode.Texture = coverTex[idx];
    var scale = new Vector2F(720 / 9 * 16, 720) / coverTex[idx].Size;
    coverNode.Scale = new Vector2F(1, 1) * (scale.X < scale.Y ? scale.X : scale.Y);
}

var cancelTokenGen = new CancellationTokenSource();
async Task moveCover(CancellationToken cancelToken)
{
    for(int i = 0; i < 720 / 9 * 16; i++)
    {
        coverNode.Position += new Vector2F(1, 0);
        if (cancelToken.IsCancellationRequested)
            break;
        await Task.Delay(10);
    }
    coverNode.Position = new Vector2F(0, 0);
    return;
}

Task animation = null;

Process? p = null;
while (Engine.DoEvents())
{
    Engine.Update();
    setCover(selectedGameIdx);

    if (Engine.Keyboard.GetKeyState(Key.Enter) == ButtonState.Push)
    {
        var info = new ProcessStartInfo("./" + games[selectedGameIdx] + "/" + games[selectedGameIdx]);
        p = Process.Start(info);
    }

    if (Engine.Keyboard.GetKeyState(Key.Left) == ButtonState.Push || Engine.Keyboard.GetKeyState(Key.A) == ButtonState.Push)
    {
        selectedGameIdx = (selectedGameIdx - 1 + games.Count) % games.Count;
        coverNode.Texture = coverTex[selectedGameIdx];
    }
    if (Engine.Keyboard.GetKeyState(Key.Right) == ButtonState.Push || Engine.Keyboard.GetKeyState(Key.D) == ButtonState.Push)
    {
        selectedGameIdx = (selectedGameIdx + 1 + games.Count) % games.Count;
        coverNode.Texture = coverTex[selectedGameIdx];
    }

    if (Engine.Keyboard.GetKeyState(Key.Escape) == ButtonState.Push && p != null)
    {
        p.Kill();
    }
}
Engine.Terminate();