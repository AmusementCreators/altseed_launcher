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

List<SpriteNode> coverNodes = new();
Vector2F idx2pos(int i)
{
    return new Vector2F(i % 3 * 720 / 9 * 16 / 3, i / 3 * 720 / 3);
}
for(int i = 0; i < coverTex.Count; i++)
{
    var coverNode = new SpriteNode();
    coverNode.Position = new Vector2F(i % 3 * 720 / 9 * 16 / 3, i / 3 * 720 / 3);
    coverNode.Texture = coverTex[i];
    var scale = new Vector2F(720 / 9 * 16, 720) / 3 / coverTex[i].Size;
    coverNode.Scale = new Vector2F(1, 1) * (scale.X < scale.Y ? scale.X : scale.Y);
    Engine.AddNode(coverNode);
    coverNodes.Add(coverNode);
}

Process? p = null;
while (Engine.DoEvents())
{
    Engine.Update();

    if (Engine.Keyboard.GetKeyState(Key.Enter) == ButtonState.Push)
    {
        var info = new ProcessStartInfo("./" + games[selectedGameIdx] + "/" + games[selectedGameIdx]);
        p = Process.Start(info);
    }

    if (Engine.Keyboard.GetKeyState(Key.Left) == ButtonState.Push || Engine.Keyboard.GetKeyState(Key.A) == ButtonState.Push)
    {
        selectedGameIdx = (selectedGameIdx - 1 + games.Count) % games.Count;
    }
    if (Engine.Keyboard.GetKeyState(Key.Right) == ButtonState.Push || Engine.Keyboard.GetKeyState(Key.D) == ButtonState.Push)
    {
        selectedGameIdx = (selectedGameIdx + 1 + games.Count) % games.Count;
    }
    if(Engine.Mouse.GetMouseButtonState(MouseButton.ButtonLeft) == ButtonState.Release)
    {
        var pos = Engine.Mouse.Position;
        selectedGameIdx = ((int)pos.X / (720 / 9 * 16 / 3)) + (int)pos.Y / (720 / 3) * 3;
        selectedGameIdx = 0 > selectedGameIdx ? 0 :(games.Count - 1 < selectedGameIdx ? games.Count-1 : selectedGameIdx);
        var info = new ProcessStartInfo("./" + games[selectedGameIdx] + "/" + games[selectedGameIdx]);
        p = Process.Start(info);
    }

    if (Engine.Keyboard.GetKeyState(Key.Escape) == ButtonState.Push && p != null)
    {
        p.Kill();
    }
}
Engine.Terminate();