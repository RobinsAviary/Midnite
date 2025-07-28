// Pixel art upscaler
// Written by Robin <3 for Midnite

uniform sampler2D tex;
uniform vec2 winSize;

void main( void ) {	
	gl_FragCoord.xy;
	//OpenGL moment
	vec2 pos = floor(gl_FragCoord) / winSize;
	pos.y = pos.y;

	gl_FragColor = texture2D(tex, pos);
}
