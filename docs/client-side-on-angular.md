# Client Side on Angular

## Installation

For Angular, we provide an [individual npm package](https://www.npmjs.com/package/devextreme-aspnet-data-nojquery) that does not depend on jQuery. Run the following command to install it:

    npm install devextreme-aspnet-data-nojquery

As DevExtreme ASP.NET Data is supposed to be used with DevExtreme widgets, make sure that you have installed the [devextreme-angular](https://github.com/DevExpress/devextreme-angular#add-to-existing-app) package in your app.

## API Reference and Usage

The API consists of a single method, `createStore`, which is described [here](docs/client-side-configuration.md#api-reference). The following example shows how to use this method to create a store for the **DataGrid** widget:

**app.component.ts**

```TypeScript
import { Component } from '@angular/core';
import { createStore } from 'devextreme-aspnet-data-nojquery'
import CustomStore from 'devextreme/data/custom_store'

@Component({
    selector: 'app-root',
    templateUrl: `
        <dx-data-grid
            [dataSource]="store">
        </dx-data-grid>
    `,
    styleUrls: ['./app.component.css']
})
export class AppComponent {
    store: CustomStore;
    constructor() {
        let serviceUrl = "http://url/to/my/service"
        this.store = createStore({
            key: "ID",
            loadUrl: serviceUrl + "/GetAction",
            insertUrl: serviceUrl + "/InsertAction",
            updateUrl: serviceUrl + "/UpdateAction",
            deleteUrl: serviceUrl + "/DeleteAction"
        })
    }
}
```

**app.module.ts**

```TypeScript
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { DxDataGridModule } from 'devextreme-angular'

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    DxDataGridModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

## See Also

- [DataGrid - Use CustomStore](https://js.devexpress.com/Documentation/Guide/Widgets/DataGrid/Use_CustomStore/)