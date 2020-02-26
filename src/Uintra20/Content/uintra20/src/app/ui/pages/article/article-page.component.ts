import { Component, ViewEncapsulation } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DeactivationGuarded, CanDeactivateGuard } from "src/app/services/general/can-deactivate.service";
import { HasDataChangedService } from "src/app/services/general/has-data-changed.service";
import { Observable } from "rxjs";
import { AddButtonService } from '../../main-layout/left-navigation/components/my-links/add-button.service';

@Component({
  selector: "article-page",
  templateUrl: "./article-page.html",
  styleUrls: ["./article-page.less"],
  encapsulation: ViewEncapsulation.None
})
export class ArticlePage implements DeactivationGuarded {
  data: any;

  constructor(
    private route: ActivatedRoute,
    private addButtonService: AddButtonService,
    private hasDataChangedService: HasDataChangedService,
    private canDeactivateService: CanDeactivateGuard,
  ) {
    this.route.data.subscribe(data => {
      this.data = data;
      this.addButtonService.setPageId(data.id);
    });
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (this.hasDataChangedService.hasDataChanged) {
      this.canDeactivateService.canDeacrivateConfirm();
    }

    return true;
  }
}
