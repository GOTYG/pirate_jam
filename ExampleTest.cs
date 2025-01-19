using GdUnit4;

namespace PirateJam;

[TestSuite]
public class ExampleTest
{
   [TestCase]
   public static void TestFoo()
   {
	  Assertions.AssertThat(5).IsEqual(5);
   } 
}