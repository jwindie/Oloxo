import processing.core.PApplet;

public class App extends PApplet {
    
    public static void main(String[] args) {
		PApplet.main("App");
    }

    @Override
    public void settings() {
        size(800, 600);  // Use P2D renderer here
        pixelDensity(displayDensity());
        smooth(8);
    }

    @Override
    public void setup() {
		rectMode(CORNER);
		ellipseMode(CORNER);
    }

    @Override
    public void draw() {
		background(100);
    }
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