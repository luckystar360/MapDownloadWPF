   M S B A M L   `   `   `  ����  MapDownloader-    'MapDownloader.Controls.MessageBoxWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$  MessageBoxWindow��5   	    � Topmost . $��14(�5   	   $��Center=�5   	    � 
ResizeMode$ NoResize=�5	   	    � WindowStyle$
 None=�5
   	      MaterialDesignPaper#���   5   	     PrimaryHueDarkBrush#���  5   	   $��4q�5   	    � WindowStartupLocation$ CenterScreen=�5   	   $	��500��5   	   $	��240��5   	    � 	Resources 5       �� MergedDictionaries+  %MaterialDesignThemes.Wpf.BundledTheme   	BaseTheme   PrimaryColor	   SecondaryColor
 �� Source�� 5   
    5        5      $ LightX�6.   $ 
DeepPurpleX�6@   $
	 LimeX�6Z   �� 5      $h
 bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?�6%   %    5      .��5      � -   5        $��MainContentControl��5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5"   
   � -   5         �Name$ TitleBackgroundPanel��5"      $��0��6   #���  6S    � ColumnDefinitions 5#      �� 5$      $
��Auto �6#   �� 5%      $��* �6#   .+�5(      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ MessageText=�6&   #���   69   $��Center=�6l   $��Center=�6�   $��6,0,6,0q�6�   �� 5)      -   5        $��TxtTitle��5)      $��1��6A   
�������$��Title��6d   $��Center=�6q   $��Medium"�6�   $��0,6,6,6q�6�   5*      �� 5,   
   -   5        $��MessageControl��5,      $��1��6F   $
��Auto=�6S   .��5-      �� -   5        $��
TxtMessage��5-   1   $	��100��6C   $��Message��6S   $
��Wrap=�6b    �� Padding$ 3q�6v   $	��Top=�6�   
���   �$��Normal"�6�   $��24,12q�6�   5.      � 50   
   $��2��6   $��24,12q�6    51      �� 52      $
��Auto �6#   �� 53      $��* �6#   .+�56      �� -   5        $��BtnCopyMessage��56   /   $��0��6d   $	��Top=�6t   $��30��6�   $��30��6�   
�����p�
������� � ToolTip$ Copy to Clipboard��6�   $
��Left=�6�   $��0q�6   * &MaterialDesignFloatingActionMiniButton#���  6  .��57        $ ContentCopy=�6*   
�����p�6=   58      �� 59      $��1��6    �� Orientation$ 
Horizontal=�6)   $��Right=�6B   .+�5:      �� -   5        $��BtnOk��5:   3   #���   6V   $��Ok��6�   $	��100��6�   $
��24,5q�6�   �� 5;      -   5        $��	BtnCancel��5;   3   $��Cancel��6^   #���   6p   $	��100��6�   
���iii�$
��24,5q�6�   $��24,0,0,0q�6�   5<      5=      5>      5?      