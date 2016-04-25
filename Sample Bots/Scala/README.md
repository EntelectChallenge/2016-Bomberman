## Scala

A Scala bot can be created by following the same procedure as the [Java bot](https://github.com/EntelectChallenge/2016-Bomberman/tree/master/Sample%20Bots/Java).

The [scala-maven-plugin](http://davidb.github.io/scala-maven-plugin/index.html) will need to be added to the `pom.xml` file, as well as the Scala library dependency:

```
<dependency>
    <groupId>org.scala-lang</groupId>
    <artifactId>scala-library</artifactId>
    <version>${scala.version}</version>
</dependency>
```

Take a look at the `pom.xml` for details of the build setup.
