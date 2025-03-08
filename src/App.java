import Core.DrawSettings;
import Core.Line;
import Core.Solid;
import processing.core.PApplet;
import processing.core.PVector;

public class App extends PApplet {
	
	Solid s = new Solid(Solid.Shape.RECTANGLE);
	Line l = new Line();
    
    public static void main(String[] args) {
		PApplet.main("App");
    }

    @Override
    public void settings() {
      	size(800, 600);
     	pixelDensity(displayDensity());
        smooth(8);
    }

    @Override
    public void setup() {
		rectMode(CORNER);
		ellipseMode(CORNER);

		s.setAnchor(new PVector(.5f, .5f));
		s.setPosition(new PVector(width/2,height/2));
		s.setSize(new PVector(200, 80));
		s.setCorners(8);
		s.getDrawSettings().setFill (hexColor("F00"));
		s.setDrawBounds(true);

		l.getDrawSettings().setStroke(Utils.DefaultColors.ORANGE);
		l.getDrawSettings().setStrokeWeight(50);
		l.setPoints(
			new PVector(width/2,height/2),
			new PVector(0,0)
		);
		l.setDrawBounds(true);
		println(l.getDrawSettings().toString());

    }

    @Override
    public void draw() {
		l.setEndPoint(new PVector(mouseX, mouseY));
		
		background(100);
		s.draw(this);
		l.draw(this);
    }

	//#region APPLET METHODS
	/**Parses colors from Hexadecimal to Processing formatted-int colors.
	 * <p>
	 * You can use 6 and 3 digit hex for rgb, or 8 and 4 digit hex for rgba.
	*/
	int hexColor (String hexColor) {

		if (hexColor.charAt(0) == '#') hexColor = hexColor.substring(1);
	

		// Parse the RGB values from the hex string
		int r=0, g=0, b=0, a=255;

		if (hexColor.length() == 6) {
			r = Integer.parseInt(hexColor.substring(0, 2), 16);
			g = Integer.parseInt(hexColor.substring(2, 4), 16);
			b = Integer.parseInt(hexColor.substring(4, 6), 16);
		}
		else if (hexColor.length() == 8) {
			r = Integer.parseInt(hexColor.substring(0, 2), 16);
			g = Integer.parseInt(hexColor.substring(2, 4), 16);
			b = Integer.parseInt(hexColor.substring(4, 6), 16);
			a = Integer.parseInt(hexColor.substring(6, 8), 16);
		}
		else if (hexColor.length() == 3) {
			r = Integer.parseInt(hexColor.substring(0, 1), 16);
			g = Integer.parseInt(hexColor.substring(1, 2), 16);
			b = Integer.parseInt(hexColor.substring(2, 3), 16);		

			r = Math.clamp(r*r,0,255);
			g = Math.clamp(g*g,0,255);
			b = Math.clamp(b*b,0,255);
		}
		else if (hexColor.length() ==4) {
			r = Integer.parseInt(hexColor.substring(0, 1), 16);
			g = Integer.parseInt(hexColor.substring(1, 2), 16);
			b = Integer.parseInt(hexColor.substring(2, 3), 16);		
			a = Integer.parseInt(hexColor.substring(3, 4), 16);		

			r = Math.clamp(r*r,0,255);
			g = Math.clamp(g*g,0,255);
			b = Math.clamp(b*b,0,255);
			a = Math.clamp(a*a,0,255);
		}
		return color(r,g,b,a);
	}
	//#endregion
}

/*
Start with a center area
-expand out by discovering land tiles then colonizing/terraforming them
  - if its a bog you have to fill the bog to make it a field to make it a farm to make food
-only buttons and icons
-as you expand outward, the obstacles get more difficult to conquer
  - its about the number of tiles not the distance
-All buildings need to occupy a tile
  -storage buildings, homes,etc
-Procedural generation
-if all your people die, its game over
-all buildings require people to work
  - require min of people to work, some will allow you to overstaff for a small buff
-resources
  -people (labor)
  -food
  -wood (basic building)
  -stone (intermediate building)
  -metal (advanced building)
  -purpa (etherial material that allows you to upgrade and get city perks)
    -very rare but very valuable
-ore veins and forests decay after a while (limited resources)
  - some are regroqable but take a LONG time to grow back so long as you didint destroy it all
-late game gets much more magical and etheral and technical 
  -might have to introduce more materials
-upgrading building increases output at cost
  -all buildings have base prod
  -each upgrade is the same price but the increase diminishes
  -at some point its more smart to build an tier II building with a higher base prod
  -some buildings may take up more than one tile
  -cannot place buildings on conflic areas or areas that are not suitable
  
-
*/