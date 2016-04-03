import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
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

        File move = new File(botDir + File.separator + "move.txt");
        try {
            if(move.createNewFile()){

                FileWriter write = new FileWriter(move);
                write.write(String.valueOf(new Random().nextInt(7)));
                write.close();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
