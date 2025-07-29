function Init()
	HelloWorld()
end

function Update()
	Draw.Clear(Color.Red)
	Draw.Line(Vec2.Zero, Vec2.NewS(50), Color.Green)
	Draw.Circle(Vec2.NewS(100), 25, Color.Yellow)
end