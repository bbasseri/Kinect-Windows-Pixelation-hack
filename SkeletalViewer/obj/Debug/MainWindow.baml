   M S B A M L   `   `   `  ����  SkeletalViewer    SkeletalViewer.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Pixelating 500px��5   	    � Icon$ SkeletalViewer.ico��6"    � WindowStartupLocation$ CenterScreen=�5   	    � SizeToContent$ WidthAndHeight=�6.    � 	Resources 5         ScrollingItemsControl �� 
TargetType �� Padding J� SnapsToDevicePixels��%�   &	        �� 5	   
   " ��68   .R�5
      �� #��z��6   # z��6P   #��z��5      #��z��6H    . ._�5      �� $
��Auto=�6   .��5      �� # z 6%   5      5      5      5      .��5      � -   5        $��Center=�5      
������  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $��insertKinectSensor��5      $	��600��6/   $��0��6;   .+�5      �� -   5        $��kinectRequiredOrEnabled��5   C   $��48(�6   $��Center=�6&   6d   �� 5      $��24(�6   $��Italic$�6&   $��Center=�69   6X   5      �� 5   
   -   5        $��viewerHolder��5      #��[  6+   $��1��6]   �� 5!   
   -   5        $��insertAnotherKinectSensor��5!      $��36(�65   $��Center=�6C   $	��600��6`   $��2��6l   $y�Hidden=�5"      6*   �� 5#   
   -   5        $!��switchToAnotherKinectSensor��5#      $��36(�67   $��Center=�6E   $	��600��6b   $��3��6n   $y�Hidden=�5$      6*   5%      5&      