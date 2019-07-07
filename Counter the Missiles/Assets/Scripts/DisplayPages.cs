using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides the functionality to navigate through help content in our "HelpScene" scene.
/// </summary>
public class DisplayPages : MonoBehaviour {

     /// <summary>
     /// The page number of the page currently being shown.
     /// </summary>
     private int currentPageNumber = 1;

     /// <summary>
     /// A list of GameObjects that are used as pages.
     /// </summary>
     private List<GameObject> pages = new List<GameObject>();

     // Start is called just before any of the Update methods is called the first time
     private void Start() {

          //grab all pages (by default, all of this GameObject's child are pages)
          for (int i = 0; i < transform.childCount; i++) {
               pages.Add(transform.GetChild(i).gameObject);
          }

          //hide all pages to prevent them from all loading up
          for (int i = 1; i < pages.Count; i++) {
               pages[i].SetActive(false);
          }

          //display the first page, which is where the user will start with
          pages[0].SetActive(true);

          //set current page number to first page
          currentPageNumber = 1;
     }

     /// <summary>
     /// Go to the previous page. Has no effect when current page is the first page.
     /// </summary>
     public void PreviousPage() {
          //to avoid out of bounds access to list, we use the following condition...
          if (currentPageNumber > 1) {
               //go previous page
               TraverseToPage(currentPageNumber - 1);
          }
     }

     /// <summary>
     /// Go to the next page. Has no effect when current page is the last page.
     /// </summary>
     public void NextPage() {
          //to avoid out of bounds access to list, we use the following condition...
          if (currentPageNumber < pages.Count) {
               //go next page
               TraverseToPage(currentPageNumber + 1);

          }
     }

     /// <summary>
     /// Go to the specified page.
     /// </summary>
     /// <param name="pageNumber"></param>
     private void TraverseToPage(int pageNumber) {

          if (pageNumber < 1 || pageNumber > pages.Count) {
               //probably invalid page number, throw exception
               throw new ArgumentException("Page number argument is out of the bounds in the pages!");
          }

          //disable current page (no need to look at it)
          pages[currentPageNumber - 1].SetActive(false);

          //traverse to specified page (the one we want to look at)
          pages[pageNumber - 1].SetActive(true);

          //our current page is now the argument's
          currentPageNumber = pageNumber;

     }
}
