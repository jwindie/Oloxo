package Obsolete;
// package Quark.Core;

// import processing.core.PApplet;
// import processing.core.PVector;

// public class Transform implements AutoCloseable {
//     private PVector translation, scale;
//     private float rotation;
//     private PApplet target;

//     public Transform (PApplet target) {
//         this.target = target;
//         translation = new PVector();
//         scale = new PVector(1,1);
//         rotation = 0;
//     }
//     public Transform (PApplet target, float x, float y, float r, float sx, float sy) {
//         this.target = target;
//         translation = new PVector(x,y);
//         scale = new PVector(sx,sy);
//         rotation = r;
//     }

//     public static void translate (PApplet target, PVector translation) {
//         translate (target, translation.x, translation.y);
//     }
//     public static void translate(PApplet target, float x, float y) {
//         target.translate(x, y);
//     }

//     public PVector getTranslation() {return translation;}
//     public float getRotation() {return rotation;}
//     public PVector getScale() {return scale;}

//     public void setTranslation(PVector translation) {
//         this.translation = translation;
//     }
//     public void setTranslation(float x, float y) {
//         this.translation = new PVector(x, y) ;
//     }
//     public void setRotation (float rotation) {
//         this.rotation = rotation;
//     }
//     public void setScale(PVector scale) {
//         this.scale = scale;
//     }
//     public void setScale(float x, float y) {
//         this.scale = new PVector(x, y) ;
//     }


//     // Transformations
//     public void push() {
//         target.pushMatrix();
//         target.scale(scale.x, scale.y);
//         target.rotate(rotation);
//         target.translate(translation.x, translation.y);
//     }

//     public void pop() {
//         target.popMatrix();
//     }

//     // The close method will handle popping the matrix when done
//     @Override
//     public void close() {
//         // Handle cleanup, for example, popping the matrix after the scope ends
//         pop();
//     }
// }