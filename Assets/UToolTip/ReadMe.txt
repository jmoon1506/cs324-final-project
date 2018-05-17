Thanks for purchase UGUIToolTip.


Get Started:----------------------------------------------------------------------------------------

- Import UGUIToolTip in your project.
- In your scene menu or where you need use, drag the prefab "ToolTip"(UToolTip -> Prefab -> ToolTip) into the canvas in scene.
- Always make sure the ToolTip is the last child of the canvas.
- Ready.

Create new Tooltip:--------------------------------------------------------------------------------

- In the Image,Text,Button,etc... to you want show the tooltip, add the script "bl_ToolTipItem.cs".
- Fill all vars:
      
       - Text          = Text to show in tooltip.
       - TakeFromImage = Get the sprite from image component of gameobject for show in tooltip(required image component).
       - Icon = Custom sprite for show in tooltip image icon.
-Ready!


Change the size of tooltip:------------------------------------------------------------------------

- In editor window change the size manually.
- then, in the tooltip script (bl_ToolTip.cs) in the root of tooltipUI,
edit the variables OffSet bearing in mind that:

 - OffSet.X = half of sizeDelta.x of the Tooltip root rectTransform.
 - OffSet.Y = half negative of sizeDelta.Y of the Tooltip root rectTransform.

-Ready!.

QA:------------------------------------------------------------------------------------------------

 -The tooltip position is wrong-----------------------------------------------------------------

      - For fix this, you need to edit the canvas -> Canvas Scaler -> Reference Resolution 
        to the same of your screen size or your target plataform build.


Contact:
Email: brinerjhonson.lc@gmail.com
Forum: http://lovattostudio.com/Forum/index.php