Sprite = {}
Sprite.mt = {
	--[[__tostring = function(t)
		return ""
	end,]]--

	__name = "Sprite",
}

Sprite.New = function(texture, position, scale, rotation, tint, origin)
	local t = {}

	setmetatable(t, Sprite.mt)

	t.texture = texture
	t.position = position or Vec2.Zero
	t.scale = scale or Vec2.NewS(1)
	t.rotation = rotation or 0
	t.tint = tint or Color.White
	t.origin = origin or Vec2.Zero
	
	return t;
end

