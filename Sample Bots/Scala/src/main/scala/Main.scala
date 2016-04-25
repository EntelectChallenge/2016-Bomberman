import java.nio.charset.StandardCharsets
import java.nio.file.{Files, Path, Paths, StandardOpenOption}

import scala.util.{Random, Try}
import scala.io.Source

object Main {

  def timer[A](f: => A): (Double, A) = {
    val t0 = System.nanoTime
    val a = f
    val t = System.nanoTime - t0
    (t / 1000000.0, a)
  }

  def fileFromPath(path: Path, filename: String): Try[Path] = Try(path.resolve(filename))

  def readFile(path: Path): Try[String] = Try {
    val source = Source.fromFile(path.toFile)(StandardCharsets.UTF_8)
    val fileContent = source.getLines().mkString
    source.close()
    fileContent
  }

  def randomMove: Try[Int] = Try(Random.nextInt(7))

  def writeFile(path: Path, output: String): Try[Unit] = Try {
    Files.write(path, output.getBytes(StandardCharsets.UTF_8), StandardOpenOption.CREATE)
  }

  def main(args: Array[String]) = {
    val result = timer {
      for {
        (botKey, botDir) <- Try((args(0), Paths.get(args(1))))
        inputFile <- fileFromPath(botDir, "state.json")
        outputFile <- fileFromPath(botDir, "move.txt")
        input <- readFile(inputFile)
        move <- randomMove
        _ <- writeFile(outputFile, move.toString)
      } yield move
    }
    println(s"Time: ${result._1} Result: ${result._2}")
  }
}

