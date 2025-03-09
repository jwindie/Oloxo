package Core;
/**PShape wrapper at its core. */

import processing.core.PShape;

public class Shape extends Entity {
    private static int instanceCount = 0;

    private PShape pShape;

    protected Shape (String name) {
        super("Shape " + (instanceCount ++ + 1));
    }
}
