import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { IDatePickerOptions } from 'src/app/feature/shared/interfaces/DatePickerOptions';
import * as moment from "moment";

export interface IPinedData {
  isPinCheked: boolean;
  isAccepted: boolean;
  pinDate: string;
}
@Component({
  selector: 'app-pin-activity',
  templateUrl: './pin-activity.component.html',
  styleUrls: ['./pin-activity.component.less']
})
export class PinActivityComponent implements OnInit {
  @Input() isPinCheked: boolean;;
  @Output() handleChange = new EventEmitter<IPinedData>();

  options: IDatePickerOptions;
  pinDate = null;
  pinedDateValue: IPinedData = {
    isPinCheked: false,
    isAccepted: false,
    pinDate: ""
  };

  constructor() { }

  ngOnInit() {
    this.pinedDateValue.isPinCheked = this.isPinCheked;
    this.options = {
      showClear: true,
      minDate: moment()
    };
    this.pinDate = moment();
  }

  onDateChange() {
    this.pinedDateValue.pinDate = this.pinDate ? this.pinDate.format() : "";
    this.handleChange.emit(this.pinedDateValue);
  }
  onAcceptedChange() {
    this.pinedDateValue.isPinCheked = this.isPinCheked;
    this.handleChange.emit(this.pinedDateValue);
  }
}