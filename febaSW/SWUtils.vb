﻿Option Explicit On

Imports SldWorks
Imports SwConst

Module SWUtils

    ''' <summary>
    ''' Make selection of the list of solid bodies
    ''' </summary>
    ''' <param name="swApp">SolidWorks application object</param>
    ''' <param name="swModel">SolidWorks document</param>
    ''' <param name="objBody">The list of bodies to be selected</param>
    ''' <remarks>
    ''' Taken from http://help.solidworks.com/2015/English/api/sldworksapi/Select_Bodies_Example_VB.htm
    ''' </remarks>
    Public Sub selectSolidBodies(ByRef swApp As SldWorks.SldWorks, _
                                 ByRef swModel As SldWorks.ModelDoc2, _
                                 ByRef objBodies As Object)

        ' %-- Variable declarations
        Dim swModExt As SldWorks.ModelDocExtension
        Dim swBody As SldWorks.Body2
        Dim strBodySelID As String
        Dim blRet As Boolean

        Dim strBodyType As String = "SOLIDBODY"

        ' %-- Instantiate Model Extension Object
        If IsNothing(objBodies) Then
            Exit Sub
        Else
            swModExt = swModel.Extension()
        End If

        ' %-- Clear Selection
        swModel.ClearSelection2(True)

        ' %-- Loop over all passed bodies and make selection by ID
        For i = LBound(objBodies) To UBound(objBodies)
            swBody = objBodies(i)
            strBodySelID = swBody.GetSelectionId()
            If i = 0 Then
                ' Set the mark to 2 for ordered operations
                blRet = swModExt.SelectByID2(strBodySelID, strBodyType, _
                                         0.0#, 0.0#, 0.0#, _
                                         False, 2, Nothing, _
                                         swSelectOption_e.swSelectOptionDefault)
            Else
                blRet = swModExt.SelectByID2(strBodySelID, strBodyType, _
                                         0.0#, 0.0#, 0.0#, _
                                         True, 2, Nothing, _
                                         swSelectOption_e.swSelectOptionDefault)
            End If

        Next i

    End Sub

    ''' <summary>
    ''' Create combined bodies feature out of selected solid bodies
    ''' </summary>
    ''' <param name="swApp">SolidWorks application object</param>
    ''' <param name="swModel">SolidWorks document</param>
    ''' <param name="objBodies">The list of bodies to be selected</param>
    ''' <param name="strFeatureName">The name of the combined bodies</param>
    ''' <author>WD41, LRS/EPFL/PSI, 2015</author>
    Public Sub combineSolidBodies(ByRef swApp As SldWorks.SldWorks, _
                                  ByRef swModel As SldWorks.ModelDoc2, _
                                  ByRef objBodies As Object, _
                                  ByVal strFeatureName As String)

        ' %-- Variable Declaration
        Dim swFeature As SldWorks.Feature
        Dim swFeatureMgr As SldWorks.FeatureManager

        ' %-- Make selection of the input bodies
        Call selectSolidBodies(swApp, swModel, objBodies)

        ' %-- Create Combined Solid Body
        swFeatureMgr = swModel.FeatureManager()
        swFeature = swFeatureMgr.InsertCombineFeature(swBodyOperationType_e.SWBODYADD, _
                                                      Nothing, Nothing)
        swFeature.Name = strFeatureName

    End Sub

    ''' <summary>
    ''' Delete a body feature 
    ''' </summary>
    ''' <param name="swApp">SolidWorks application object</param>
    ''' <param name="swModel">SolidWorks document object</param>
    ''' <param name="strFeatureName">The name of the combine feature to be deleted</param>
    ''' <remarks></remarks>
    Public Sub deleteCombinedFeature(ByRef swApp As SldWorks.SldWorks, _
                                     ByRef swModel As SldWorks.ModelDoc2, _
                                     ByVal strFeatureName As String)

        ' %-- Variable declarations
        Dim swModExt As SldWorks.ModelDocExtension
        Dim blRet As Boolean

        Dim strBodyFeature As String = "BODYFEATURE"

        swModExt = swModel.Extension()

        ' %-- Select feature
        swModel.ClearSelection2(True)
        blRet = swModExt.SelectByID2(strFeatureName, strBodyFeature, _
                                     0.0#, 0.0#, 0.0#, _
                                     False, 0, Nothing, _
                                     swSelectOption_e.swSelectOptionDefault)

        If Not blRet Then
            Exit Sub
        Else
            ' %-- Delete feature
            swModel.EditDelete()
        End If

    End Sub

End Module
