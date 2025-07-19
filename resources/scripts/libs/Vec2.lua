-- Namespaces
Vec2 = {}
-- Special syntax functionality
Vec2.mt = {
	__add = function(a,b)
		return Vec2.New(a.x + b.x, a.y + b.y)
	end,

	__sub = function(a,b)
		return Vec2.New(a.x - b.x, a.y - b.y)
	end,

	__mul = function(a,b)
		return Vec2.New(a.x * b.x, a.y * b.y)
	end,

	__div = function(a,b)
		return Vec2.New(a.x / b.x, a.y / b.y)
	end,

	__tostring = function(t)
		return "{x: "..tostring(t.x)..", y: "..tostring(t.y).."}"
	end,

	__eq = function(a,b)
		return (a.x == b.x and a.y == b.y)
	end,

	__len = 2,

	__name = "Vec2",
}

Vec2.New = function(x, y)
	local r = {}

	setmetatable(r, Vec2.mt)

	r.x = x or 0;
	r.y = y or 0;
	
	return r;
end

Vec2.NewS = function(xy)
	return Vec2.New(xy, xy)
end

Vec2.Zero = Vec2.NewS(0)
Vec2.Up = Vec2.New(0, -1)
Vec2.Down = Vec2.New(0, 1)
Vec2.Left = Vec2.New(-1, 0)
Vec2.Right = Vec2.New(1, 0)