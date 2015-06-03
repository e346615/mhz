Imports System.Xml
Imports System.Text.RegularExpressions
Imports HtmlAgilityPack

Module Module1

    Sub Main()
        Dim web As New HtmlWeb
        Dim doc As New HtmlDocument
        Dim re As Regex = New Regex("hilosophy")
        Const website = "http://en.wikipedia.org/wiki/Language"
        Dim websiteUri = New Uri(website)

        doc = web.Load(website)
        For Each link As HtmlNode In doc.DocumentNode.SelectNodes("//a")
            If (re.IsMatch(link.InnerText)) Then
                Dim s As New HtmlDocument
                s.LoadHtml(link.OuterHtml)
                For Each href As HtmlNode In s.DocumentNode.SelectNodes("//a[@href]")
                    Dim t As String = href.Attributes("href").Value
                    Dim u As Uri = New Uri(t, UriKind.RelativeOrAbsolute)
                    If Not (u.IsAbsoluteUri) Then
                        u = New Uri(websiteUri, u)
                    End If

                    Console.WriteLine(u.AbsoluteUri)
                Next
            End If
        Next
        Console.ReadKey()
    End Sub

End Module


