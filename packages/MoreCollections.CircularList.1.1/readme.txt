var list = new string[]{"one", "two", "three"}.ToList();
  
var circularList = list.ToCircularList();
Console.WriteLine(circularList[-1]); //outputs "three"
Console.WriteLine(circularList[6]); //outputs "one"