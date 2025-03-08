package Utils;
public class HexRGB {
    public static void main(String[] args) {
        String hexColor = "#abb3b2";  // Hexadecimal color string

        // Remove the "#" if it's included in the hex string
        if (hexColor.startsWith("#")) {
            hexColor = hexColor.substring(1);
        }

        // Parse the RGB values from the hex string
        int r = Integer.parseInt(hexColor.substring(0, 2), 16);  // Red
        int g = Integer.parseInt(hexColor.substring(2, 4), 16);  // Green
        int b = Integer.parseInt(hexColor.substring(4, 6), 16);  // Blue

        // Print the RGB values
        System.out.println("RGB Values: ");
        System.out.println("Red: " + r);
        System.out.println("Green: " + g);
        System.out.println("Blue: " + b);
    }
}