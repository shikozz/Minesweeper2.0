   M S B A M L   `   `   `  ����C  ?saperdun, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:saperdunsaperdun  5   	       saperdun.MSGBox   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:saperdun     5   	     � WindowStyle$
  None=�5   	    � 
ResizeMode$ NoResize=�5	   	    � SizeToContent$ WidthAndHeight=�5
   	    � Title$ MSGBox��5   	   $	��100��6   $	��200��6%   $	��300��61   $	��100��6@   $	��200��6P      MyWindowStyle#��[  6_   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      CaptionHeight$ 0Q�6      GlassFrameThickness$ 0,0,0,1q�6)      ResizeBorderThickness$ 7q�6G      CornerRadius$ 0��6a   	   NonClientFrameEdges$
	 none=�6r   .��5      � 
 � RowDefinitions
 5   
   �� 5      $
��auto �6   �� 5      .+�5   
   � -   5        $��	TitleGrid��5      $��0��6      MyHeaderGridStyle#��[ 6-    � ColumnDefinitions 5      �� 5      $��26 �6#   �� 5      $��* �6#   �� 5      $
��auto �6#   �� 5      $
��auto �6#   �� 5      $
��auto �6#   .+�5      �� -   5        $��	TitleText��5      $��1��6   $��Center=�5!      $��Center=�6"     HeaderTextBlock#��[ 5"      $��12(�6.    �� Padding$ 4,0,4,0q�6<   $��Regular"�6N   ��5       ��  �� Path$ Title� �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType   saperdun.MainWindow"  5"   N   �� 5#      -   5        $��bClose��5#      $��4��6#     SysButtonClose#��[ 63   $��0,0,5,10q�6^   6p   5$      � 5%   
   $��1��6   .+�5&      �� .+�5'      �� -   5         ��Name$ MessageTextBlock��5'      $
��Wrap=�66   $��10q�6J     TextBlockStyle#��[ 6V   �� 5(       �� Orientation$ 
Horizontal=�6   $��Right=�66   $��10q�6R   .+�5)      �� -   5         ��Name$ OKButton��5)      $��OK��6/   $��75��6<     BaseButtonStyle#��[ 6^   5*      5+      5,      5-      5.      