   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5      @    :Microsoft.Samples.Kinect.WpfViewers.KinectDiagnosticViewer   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @kt1clr-namespace:Microsoft.Samples.Kinect.WpfViewers     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      $	��445��5      $	��829��6,   .��5      � $
��Left=�6   $	��Top=�6-   .+�5	   
   �� .+�5
      �� -   5        $��image1��5
   -   $	��401��6   $	��509��6!   $
��Fill=�6;   $E��?/Microsoft.Samples.Kinect.WpfViewers;component/Images/Start.jpg��6K   5      �� 5   
   -   5        $��skeletonPanel��5      $	��409��6+   $	��295��68   .+�5      �� -   5        $��skeletonCanvas��5   .   $	��244��6   $	��142��6!   
���   �}�. $��Right=�6k   $	��Top=�6�   �� 5        �� Orientation$  Vertical=�6   $��80��60   $	��247��6<   $��Right=�6H   .+�5      �� $��12(�6   $��Center=�6*   	Framerate6R   �� 5      $  Vertical=�6   
�������
���   �$��34��6d   $	��146��6p   .+�5      �� -   5        $��	frameRate��5   .   $��10(�6    $��Center=�6?   ��6\   ��  �� ElementName$ kinectDepthViewer�� �� Path$ 	FrameRate��� 5      $��10(�6    $��Center=�6.   FPS6P   5      �� 5      $��12(�6   $��Center=�6*   Status6O   �� 5      
�������
���   �.+�5      �� -   5        $��kinectStatus��5   .   $��10(�6    $��Not Connected��6B   $��Center=�6W   5      5      �� 5      -   5        $��button1��5   1   $��Login��6   $��23��6%   $	��133��6@   $��5q�6b   �� 5      -   5        $��txtDebug��5   #   $	��127��6   $	��289��63    �� 
IsReadOnly . $ �� HorizontalScrollBarVisibility$
 Auto=�6Q   " �� VerticalScrollBarVisibility$
 Auto=�6v   �� 5      -   5        $��lblLogin��5       $��28��6   6    5      5      5      