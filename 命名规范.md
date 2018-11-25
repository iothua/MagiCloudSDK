# MFrameworkCore行为准则

## 开发提交准则

每次需要更改项目代码时，必须拉取新的分支。

## 脚本命名规范
说明出自<https://www.jianshu.com/p/dc26cb8ffcb9>


* 用Pascal规则来命名属性、方法、事件和类名
```cs
public class HelloWorld
{
    public void SayHello(string name)
    {
    }
}
```
Pascal规则是指名称中单词的首字母大写 ,如EmployeeSalary、 ConfimationDialog、PlainTextEncoding。

* 用Camel规则来命名成员变量、局部变量和方法的参数
```cs
public class Product
{
    private string productId;
    private string productName;
    
    public void AddProduct(string productId,string productName)
    {
    }
}

```
Camel规则类似于Pascal规则 ,但名称中第一个单词的首字母不大写 ,如employeeSalary、 confimationDialog、plainTextEncoding。

* 不要使用匈牙利命名法
不要给成员变量加任何前缀（如、m、s_等等）。如果想要区分局部变量和成员变量，可以使用this关键字。
* 不要将常量或者只读变量的变量名全部大写，而使用Pascal规则来命名
```cs
// Correct
public static const string ShippingType = "DropShip";
    
// Avoid
public static const string SHIPPINGTYPE = "DropShip";
```
* 接口的名称一般以大写I作前缀
```cs
public interface IConvertible
{
    byte ToByte();
}
```
* 自定义的属性以Attribute结尾
```cs
public class TableAttribute:Attribute
{
}
```
* 自定义的异常以Exception结尾
```cs
public class NullEmptyException:Exception
{

}
```
* 局部变量的名称要有意义
```cs
不要直接用用i,j,k,l,m,n,x,y,z等做变量名，但for循环除外
```
* 代码分块
所有的成员变量声明在类的顶端，用一个换行把它和方法分开。同时可以使用成对的#region...#endregion标记，方便折叠。

* 布尔型变量或者方法一般可以用is、can、has或者should做前缀。如，isFinished, canWork等。
* 一般C#的编码风格要求花括号{另起一行，不要直接跟在类名和方法后面。
```cs
public Sample()
{
    // TODO: 在此处添加构造函数逻辑
}
```
* 可以用缩写作为UI元素的前缀
常见UI组件的一般缩写形式：
```cs
Label --> lbl、Text --> txt、Button --> btn
Image --> img、 Widget --> Wgt、 List --> lst、CheckBox --> chk
Hyperlink --> lnk、Panel --> pnl、Table --> tab
ImageButton --> imb

```
* 判断条件是一个布尔变量时不要使用==进行条件判断
```cs
// 不友好的写法
private bool isFinished = true;
if(isFinished == true)
{
    // ...
}

// 正确的写法
private bool isFinished = true;
if(isFinished)
{
    // ...
}
```
* 慎用缩写
变量名是一个单词的尽量不要缩写，多单词组成的变量名可适当缩写。
* 在类的顶部声明所有的成员变量，静态变量声明在最前面
```cs
// Correct
public class Account
{
    public static string BankName;
    public static decimal Reserves;
    
    public string Number {get; set;}
    public DateTime DateOpened {get; set;}
    public DateTime DateClosed {get; set;}
    public decimal Balance {get; set;}
    
    // Constructor
    public Account()
    {
        // ...
    }
}
```
* 方法的书写规范
如果一个方法超过25行，就需要考虑是否可以重构和拆分成多个方法。方法命名要见名知意，好的方法名可以省略多余的注释。方法功能尽量单一。
## 文件命名规范
