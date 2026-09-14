import { InsuranceTypePair } from "./InsuranceTypePair";
import { ModelSpec } from "./ModelSpec";

export class quoteDetail{
    makes: Array<string> = [];
    models: Array<ModelSpec> = [];
    insuranceTypes: Array<InsuranceTypePair> = [];
}