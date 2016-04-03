import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.util.Random;

/**
 * Created by hennie.brink on 2016/02/07.
 */
public class Main {
    public static void main(String... args){

        String botKey = args[0];
        String botDir = args[1].replace("\"","");

        System.out.println("Bot Key: " + botKey);
        System.out.println("Bot Dir: " + botDir);

        File move = new File(botDir + File.separator + "state.json");

        if(move.exists()){

            try {
                Files.readAllLines(move.toPath(), Charset.defaultCharset());
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
}
