import java.awt.Color;

import Quark.Button;
import Quark.Line;
import Quark.Solid;
import processing.core.PApplet;
import processing.core.PVector;

public class App extends PApplet {

	Solid solid;
	Solid solid2;
	Line line;
    int shape = 0, max = 4;

    Button[] buttons;
    
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

		solid = new Solid(Solid.Shape.RECTANGLE);
		solid.setFill (color ( 255, 0,0));
		solid.setPosition(new PVector(width/2, height/2));
		solid.setAnchor (new PVector(.5f, .5f)); 
		solid.setSize(new PVector(200,200));
		solid.setCorners(0,4,8,16);
		solid.setStrokeWeight(10);

        solid2 = new Solid(Solid.Shape.ELLIPSE);
		solid2.setFill (color ( 0, 255,0));
		solid2.setPosition(new PVector(20, 40));
		solid2.setSize(new PVector(100,100));

	    line = new Line (new PVector(40, 40), new PVector(width-40, height- 40)); 
		line.setStroke (color (0,200,0));
		line.setStrokeWeight(10);

        buttons = new Button[2];
        float buttonSpread = 300;
        for (int i = 0 ; i < buttons.length; i ++) {
            buttons[i] = new Button(Solid.Shape.RECTANGLE);
            buttons[i].setFill (color ( 255, 0,0));
            buttons[i].setHoveredFill(color (255));
            buttons[i].setPosition(new PVector(
                (width/2) + (buttonSpread*i) - (buttonSpread/2),
                height/2
            ));
            buttons[i].setAnchor (new PVector(.5f, .5f)); 
            buttons[i].setSize(new PVector(200,80));
            buttons[i].setCorners(8);
        }
        buttons[0].setDisabled (true);
	}

    @Override
    public void draw() {
		background(100);

        switch (shape) {
            case 0:
                fill (255, 0, 0);
                textAlign(CENTER,CENTER);
                textSize(50);
                text ("Hello World!", width/2,height/2); 
            break;
            case 1:
                    line.setEnd(new PVector(mouseX, mouseY));
                    line.draw(this);
            break;
            case 2: 
                solid.setPosition(new PVector (mouseX,mouseY));
                solid.draw(this);
            break;
            case 3:
                PVector size = PVector.sub(new PVector(mouseX, mouseY), solid2.getAnchoredPosition());
                solid2.setSize(size);
                solid2.draw (this);
            break;
        }
        background(100);
        for (int i = 0 ; i < buttons.length; i ++) {
            buttons[i].onUpdate(mouseX, mouseY);
            buttons[i].draw(this);
        }
    }


    int JavaColorToInt (Color c) {
        int r = c.getRed();
        int g = c.getGreen();
        int b = c.getBlue();
        int a = c.getAlpha();
        return color (r, g, b, a);
    }

    @Override
    public void mousePressed() {
        shape ++;
        if (shape == max) shape = 0;
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