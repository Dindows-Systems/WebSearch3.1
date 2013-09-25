<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChartingIQ.aspx.cs" Inherits="ChartingIQ" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Charting&nbsp;the&nbsp;query&nbsp;distribution&nbsp;along&nbsp;the&nbsp;1/Variance&nbsp;axis.</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <webChart:ChartControl ID="ChartControl" runat="server" ChartPadding="30" GridLines="Both"
                Height="500px" ShowTitlesOnBackground="False" TopPadding="20" Width="800px" YCustomEnd="0"
                YCustomStart="0" YValuesInterval="0">
                <YAxisFont StringFormat="Far,Near,Character,LineLimit" />
                <XTitle Font="Tahoma, 8pt, style=Bold" ForeColor="SteelBlue" StringFormat="Center,Far,Character,LineLimit"
                    Text="1 / Variance" />
                <PlotBackground Angle="90" EndPoint="100, 500" ForeColor="#FFFFC0" Type="LinearGradient" />
                <ChartTitle Font="Tahoma, 10pt, style=Bold" ForeColor="White" StringFormat="Center,Near,Character,LineLimit"
                    Text="Overall View of Query Distribution along I(Q) Axis" />
                <Border Color="LightSteelBlue" />
                <XAxisFont StringFormat="Center,Near,Character,LineLimit" />
                <Background Angle="90" Color="CornflowerBlue" EndPoint="100, 500" ForeColor="#C0FFC0"
                    Type="LinearGradient" />
                <YTitle Font="Tahoma, 8pt, style=Bold" ForeColor="SteelBlue" StringFormat="Near,Near,Character,DirectionVertical"
                    Text="# of queries" />
                <Legend Position="Bottom" Width="35"></Legend>
            </webChart:ChartControl>
        </div>
    </form>
</body>
</html>
